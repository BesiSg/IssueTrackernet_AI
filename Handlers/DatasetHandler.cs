using Atlassian.Jira;
using BesiAI;
using JiraWrapper;
using System.Reflection;
using Utility;
using Utility.Lib.Ticket;

namespace Handlers
{
    public class DataSetHandler : BaseUtility
    {
        private Dataset dataset;
        public DataSetHandler(Dataset dataset)
        {
            this.dataset = dataset;
        }
        public Dataset Get => this.dataset;
        private JIRA _jiraHandler;
        private AIHandler _aiHandler;
        public event EventHandler DatasetChanged;
        public event EventHandler<List<string>> PriorityChanged;
        public void SetPrio()
        {
            var ListPrio = new List<string>();
            var tsgsds = this.dataset.PriorityList.Split(',', '+', '*', '/').ToList();
            tsgsds.ForEach(x => x.Replace(" ", string.Empty));
            tsgsds.ForEach(x => x.ToLower());
            tsgsds.ForEach(x =>
            {
                if (x.Contains("tsgsd-"))
                    ListPrio.Add(x);
                else
                    ListPrio.Add("tsgsd-" + x);
            });
            UpdatePriority(ListPrio).Wait();
            this.DatasetChanged?.Invoke(this, null);
        }
        public void Clear()
        {
            this.dataset.Issues.Clear();
            this.DatasetChanged?.Invoke(this, null);
        }
        private Task<ErrorResult> UpdatePriority(List<string> source)
        {
            return Task.Run(() =>
            {
                ClearErrorFlags();
                try
                {
                    lock (this.dataset.Issues)
                    {
                        if (source.Count == 0) return Result;
                        this.dataset.Issues.Values.Where(x => source.Contains(x.Summary?.Value?.ToLower())).ToList().ForEach(x => x.HotPriority = "True");
                        this.dataset.Issues.Values.Where(x => !source.Contains(x.Summary?.Value?.ToLower())).ToList().ForEach(x => x.HotPriority = "False");
                    }
                }
                catch (Exception ex)
                {
                    CatchException(ex);
                }
                return Result;
            });
        }
        public Task<ErrorResult> SingleAdd(TicketInfo source)
        {
            return Task.Run(() =>
            {
                ClearErrorFlags();
                try
                {
                    lock (this.dataset.Issues)
                    {
                        if (source?.Key == null) return Result;
                        if (this.dataset.Issues.ContainsKey(source.Key.Value))
                            this.dataset.Issues[source.Key.Value].UpdateStatus(source);
                        else
                            this.dataset.Issues[source.Key.Value] = source;
                    }
                }
                catch (Exception ex)
                {
                    CatchException(ex);
                }
                return Result;
            });
        }
        public Task<ErrorResult> Add(IEnumerable<TicketInfo> source)
        {
            return Task.Run(() =>
            {
                ClearErrorFlags();
                try
                {
                    var tasks = new List<Task<ErrorResult>>();
                    foreach (var ticket in source)
                    {
                        //this.SingleAdd(ticket).Wait();
                        tasks.Add(this.SingleAdd(ticket));
                    }
                    Task.WaitAll(tasks.ToArray());
                    this.DatasetChanged?.Invoke(this, null);
                }
                catch (Exception ex)
                {
                    CatchException(ex);
                }
                return Result;
            });
        }
        public IEnumerable<TicketInfo> GetTicketInfos()
        {
            return this.dataset.Issues?.Values;
        }
        public Task<ErrorResult> UpdatefromJira()
        {
            return Task.Run(() =>
            {
                ClearErrorFlags();
                try
                {
                    if (_jiraHandler == null) _jiraHandler = new JIRA(dataset.PersonalAccessToken);
                    var issues = _jiraHandler.GetIssues(JiraWrapper.Project.All, dataset.LastUpdateDate);
                    var issuelist = new List<TicketInfo>();
                    var listoftasks= new List<Task<ErrorResult>>();
                    foreach (var issue in issues)
                    {
                        listoftasks.Add(FetchingofProperties(issue));
                    }
                    Task.WaitAll(listoftasks.ToArray());
                    this.dataset.LastUpdateDate = DateTime.Now;
                }
                catch (Exception ex)
                {
                    CatchException(ex);
                }
                return Result;
            });
        }

        private Task<ErrorResult> FetchingofProperties(Issue sdrissue)
        {
            return Task.Run(() =>
            {
                ClearErrorFlags();
                try
                {
                    var tsgsdissue = _jiraHandler.GetTSGSDviaSDR(sdrissue);
                    if (tsgsdissue == null) return Result;
                    var ticketinfo = (TicketInfo)Activator.CreateInstance(typeof(TicketInfo));
                    var allproperties = ticketinfo.GetType().GetProperties();
                    foreach (var property in allproperties)
                    {
                        var attr = property.GetCustomAttribute<TextAttribute>();
                        if (attr == null | attr?.UpdatableFromJira == false) continue;
                        var propertyInfodestination = ticketinfo.GetType().GetProperty(property.Name);
                        var propertyInfosource = sdrissue.GetType().GetProperty(property.Name);
                        var content = string.Empty;
                        if (!attr.Attribute.Equals("Comments"))
                            content = propertyInfosource == null ? sdrissue[attr.Attribute]?.ToString() : propertyInfosource.GetValue(sdrissue)?.ToString();

                        if (attr.Attribute.Equals("Assignee"))
                        {
                            propertyInfodestination.SetValue(ticketinfo, sdrissue.AssigneeUser?.DisplayName ?? string.Empty);
                        }
                        else if (attr.Attribute.Equals("Comments"))
                        {
                            propertyInfodestination.SetValue(ticketinfo, _jiraHandler.GetBeautifulSoupComments(tsgsdissue), null);
                        }
                        else if (propertyInfodestination.PropertyType.Name == "String")
                        {
                            propertyInfodestination.SetValue(ticketinfo, content ?? string.Empty, null);
                        }
                        else if (propertyInfodestination.PropertyType.Name == "sEntry")
                        {
                            content = propertyInfosource.GetValue(sdrissue).ToString();
                            var ticketname = string.Empty;

                            if (attr.Attribute.Equals("Key"))
                            {
                                ticketname = content;
                            }
                            else if (attr.Attribute.Equals("Summary"))
                            {
                                ticketname = tsgsdissue.Key.ToString();

                                PropertyInfo propertyInfoTSG = ticketinfo.GetType().GetProperty("TSGSD");
                                propertyInfoTSG.SetValue(ticketinfo, content, null);
                            }
                            if (string.IsNullOrEmpty(ticketname))
                                return Result;
                            var link = new sEntry();
                            link.Uri = (new Uri(new Uri("https://jira.besi.com/browse/"), ticketname)).ToString();
                            link.Value = ticketname;
                            propertyInfodestination.SetValue(ticketinfo, link, null);
                        }
                        else if (propertyInfodestination.PropertyType.Name == "DateTime")
                            propertyInfodestination.SetValue(ticketinfo, propertyInfosource.GetValue(sdrissue), null);
                    }
                    this.SingleAdd(ticketinfo);
                }
                catch (Exception ex)
                {
                    CatchException(ex);
                }
                return Result;
            });
        }
    }
}
