using Atlassian.Jira;
using RestSharp;
using HtmlAgilityPack;
using Utility;

namespace JiraWrapper
{
    public class JIRA
    {
        private Jira _jira;
        public JIRA(string PAT)
        {
            _jira = Jira.CreateRestClient("https://jira.besi.com", null, null);
            _jira.RestClient.RestSharpClient.Authenticator = new PATTokenAuthentication(PAT);
        }
        public IEnumerable<Issue> GetIssues(Project project, DateTime changeDate)
        {
            var issues = new List<Issue>();
            var projectJql = project == Project.All ? "FSL, XHD, NCP, AMS-X" : project.ToString();
            if (changeDate == null || changeDate.CompareTo(new DateTime()) == 0) changeDate = new DateTime(2019, 01, 01);
            var changeDateJql = changeDate.ToString("yyyy-MM-dd");
            int startAt = 0;
            int total = 0;
            int issuesPerRequest = 100;
            var initialtest = _jira.Issues.GetIssuesFromJqlAsync($"project in ({projectJql}) AND issuetype = \"Service Desk Request\" AND \"Pkg Department\" in (Software,Motion,Vision, \"Process Technology\") AND (updatedDate > {changeDateJql}  OR createdDate > {changeDateJql})", 1, 0).Result;
            var totalissues = initialtest.TotalItems;
            var listoftasks = new List<Task<IPagedQueryResult<Issue>>>();
            do
            {
                listoftasks.Add(_jira.Issues.GetIssuesFromJqlAsync($"project in ({projectJql}) AND issuetype = \"Service Desk Request\" AND \"Pkg Department\" in (Software,Motion,Vision, \"Process Technology\") AND (updatedDate > {changeDateJql}  OR createdDate > {changeDateJql})", issuesPerRequest, startAt));
                startAt = startAt + issuesPerRequest;
            } while (startAt < totalissues);
            Task.WaitAll(listoftasks.ToArray());
            listoftasks.ForEach(x => issues.AddRange(x.Result));
            return issues;
        }
        public IEnumerable<Comment> GetComments(string ticket)
        {
            var issue = _jira.Issues.GetIssueAsync(ticket).Result;
            if (issue != null)
                return issue.GetCommentsAsync().Result;
            return null;
        }
        public IEnumerable<Comment> GetComments(Issue issue)
        {
            if (issue != null)
                return issue.GetCommentsAsync().Result;
            return null;
        }
        public Issue GetTSGSDviaSDR(Issue issue)
        {
            if (issue?.Type?.Name != "Service Desk Request") return null;
            var linkedissues = issue.GetIssueLinksAsync().Result;
            foreach (var linkedissue in linkedissues)
            {
                if (linkedissue.LinkType.Name != "Problem/Incident") continue;
                if (issue.Summary.Contains(linkedissue.OutwardIssue.Key.ToString())) return linkedissue.OutwardIssue;
                if (issue.Summary.Contains(linkedissue.InwardIssue.Key.ToString())) return linkedissue.InwardIssue;
            }
            return null;
        }
        public string GetBeautifulSoupComments(Issue issue)
        {
            var summary = issue.Description.GetBeautifulSoup();
            var listofcomments = GetComments(issue);
            foreach (var comment in listofcomments)
            {
                var orga = comment.AuthorUser.DisplayName.Contains('@') ? comment.AuthorUser.DisplayName.Split('@')[0] : "Besi";
                var text = comment.Body.GetBeautifulSoup();
                summary += $"\n-----\nComment from {comment.AuthorUser.DisplayName} of {orga}: {text}";
            }
            return summary;
        }
    }
    public enum Project
    {
        AMSX,
        FSL,
        NCP,
        XHD,
        All,
    }
    public class PATTokenAuthentication : RestSharp.Authenticators.IAuthenticator
    {
        private string _token;
        public PATTokenAuthentication(string token)
        {
            _token = token;
        }

        public void Authenticate(IRestClient client, IRestRequest request)
        {
            request.AddHeader("Authorization", $"Bearer {_token}");
        }
    }
}
