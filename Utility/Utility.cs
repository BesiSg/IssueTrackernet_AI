
using OfficeOpenXml;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Utility
{
    public class BindingProxy : Freezable
    {
        #region Overrides of Freezable

        protected override Freezable CreateInstanceCore()
        {
            return new BindingProxy();
        }

        #endregion

        public object Data
        {
            get { return (object)GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }

        public static readonly DependencyProperty DataProperty =
            DependencyProperty.Register("Data", typeof(object),
                                         typeof(BindingProxy));
    }


    [Serializable]
    public abstract class aSaveable : BaseUtility
    {

    }
    public class RecipeBaseUtility : B_Utility, INotifyPropertyChanged, System.ICloneable
    {
        #region MIT binding
        private Dictionary<string, object> propertyValueStorage;
        #region GetProperty
        protected T GetValue<T>(Expression<Func<T>> property)
        {
            var lambdaExpression = property as LambdaExpression;

            if (lambdaExpression == null)
            {
                throw new ArgumentException(@"Lambda expression return value can't be null", "property");
            }

            string propertyName = GetPropertyName(lambdaExpression);
            return GetValue<T>(propertyName);
        }

        private static string GetPropertyName(LambdaExpression lambdaExpression)
        {
            MemberExpression memberExpression;

            if (lambdaExpression.Body is UnaryExpression)
            {
                var unaryExpression = lambdaExpression.Body as UnaryExpression;
                memberExpression = unaryExpression.Operand as MemberExpression;
            }
            else
            {
                memberExpression = lambdaExpression.Body as MemberExpression;
            }

            return memberExpression == null ? null : memberExpression.Member.Name;
        }

        private T GetValue<T>(string propertyName)
        {
            object value;
            if (propertyValueStorage == null)
                propertyValueStorage = new Dictionary<string, object>();
            if (propertyValueStorage.TryGetValue(propertyName, out value))
            {
                return (T)value;
            }

            return default(T);
        }
        #endregion
        #region SetProperty
        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1126:PrefixCallsCorrectly",
                Justification = "Required as Equals can handle null refs.")]
        protected bool SetValue<T>(Expression<Func<T>> property, T value, bool compareBeforeTrigger = true)
        {
            var lambdaExpression = property as LambdaExpression;

            if (lambdaExpression == null)
            {
                throw new ArgumentException(@"Lambda expression return value can't be null", "property");
            }

            string propertyName = GetPropertyName(lambdaExpression);
            var storedValue = GetValue<T>(propertyName);

            if (compareBeforeTrigger)
            {
                if (typeof(T) == typeof(Uri) && storedValue != null)
                {
                    if (Equals(storedValue.ToString(), value.ToString()))
                        return false;
                }
                else
                {
                    if (Equals(storedValue, value))
                        return false;
                }
            }
            propertyValueStorage[propertyName] = value;
            OnPropertyChanged(propertyName);

            return true;
        }
        #endregion
        #endregion
        #region inotifypropertychanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            RecipeNoticeHandler.RecipeChanged = true;
        }

        protected virtual void OnPropertyChanged<T>(Expression<Func<T>> raiser)
        {
            var propName = ((MemberExpression)raiser.Body).Member.Name;
            OnPropertyChanged(propName);
        }

        protected bool Set<T>(ref T field, T value, [CallerMemberName] string name = null)
        {
            //if ( !EqualityComparer<T>.Default.Equals( field, value ) )
            {
                field = value;
                OnPropertyChanged(name);
                return true;
            }
            //return false;
        }
        #endregion
    }
    public abstract class BaseUtility : B_Utility, INotifyPropertyChanged
    {
        #region binding
        private Dictionary<string, object> propertyValueStorage;
        #region GetProperty
        protected T GetValue<T>(Expression<Func<T>> property)
        {
            var lambdaExpression = property as LambdaExpression;

            if (lambdaExpression == null)
            {
                throw new ArgumentException(@"Lambda expression return value can't be null", "property");
            }

            string propertyName = GetPropertyName(lambdaExpression);
            return this.GetValue<T>(propertyName);
        }

        private static string GetPropertyName(LambdaExpression lambdaExpression)
        {
            MemberExpression memberExpression;

            if (lambdaExpression.Body is UnaryExpression)
            {
                var unaryExpression = lambdaExpression.Body as UnaryExpression;
                memberExpression = unaryExpression.Operand as MemberExpression;
            }
            else
            {
                memberExpression = lambdaExpression.Body as MemberExpression;
            }

            return memberExpression == null ? null : memberExpression.Member.Name;
        }

        private T GetValue<T>(string propertyName)
        {
            object value;
            if (this.propertyValueStorage == null)
                this.propertyValueStorage = new Dictionary<string, object>();
            if (this.propertyValueStorage.TryGetValue(propertyName, out value))
            {
                return (T)value;
            }

            return default(T);
        }
        #endregion
        #region SetProperty
        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1126:PrefixCallsCorrectly",
                Justification = "Required as Equals can handle null refs.")]
        protected bool SetValue<T>(Expression<Func<T>> property, T value, bool compareBeforeTrigger = true)
        {
            var lambdaExpression = property as LambdaExpression;

            if (lambdaExpression == null)
            {
                throw new ArgumentException(@"Lambda expression return value can't be null", "property");
            }

            string propertyName = GetPropertyName(lambdaExpression);
            var storedValue = this.GetValue<T>(propertyName);

            if (compareBeforeTrigger)
            {
                if (typeof(T) == typeof(Uri) && storedValue != null)
                {
                    if (Equals(storedValue.ToString(), value.ToString()))
                        return false;
                }
                else
                {
                    if (Equals(storedValue, value))
                        return false;
                }
            }
            this.propertyValueStorage[propertyName] = value;
            this.OnPropertyChanged(propertyName);

            return true;
        }
        #endregion
        #endregion
        #region inotifypropertychanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected virtual void OnPropertyChanged<T>(Expression<Func<T>> raiser)
        {
            var propName = ((MemberExpression)raiser.Body).Member.Name;
            this.OnPropertyChanged(propName);
        }

        protected bool Set<T>(ref T field, T value, [CallerMemberName] string name = null)
        {
            //if ( !EqualityComparer<T>.Default.Equals( field, value ) )
            {
                field = value;
                this.OnPropertyChanged(name);
                return true;
            }
            //return false;
        }
        #endregion
    }
    public abstract class B_Utility : BindableBase, System.ICloneable
    {
        #region Errorformatting
        protected string FormatErrMsg(string Name, Exception ex)
        {
            var TypeName = this.GetType().Name;
            var st = new StackTrace();
            var sf = st.GetFrame(1);
            var MethodNameArr = sf.GetMethod().Name.Split('<', '>');
            var MethodName = MethodNameArr.Length == 3 ? MethodNameArr[1] : MethodNameArr[0];
            string fullMethodName = $"{TypeName}:{MethodName}:,";
            return $"{Name}:{fullMethodName}{ex.Message}";
        }
        protected string FormatErrMsg2(string Name, Exception ex)
        {
            var TypeName = this.GetType().Name;
            var st = new StackTrace();
            var sf = st.GetFrame(2);
            var MethodNameArr = sf.GetMethod().Name.Split('<', '>');
            var MethodName = MethodNameArr.Length == 3 ? MethodNameArr[1] : MethodNameArr[0];
            string fullMethodName = $"{TypeName}:{MethodName}:,";
            return $"{Name}:{fullMethodName}{ex.Message}";
        }
        #endregion
        #region Shared error handling
        protected ErrorResult Result { get; private set; } = new ErrorResult();
        protected void ThrowError(string ErrorMessage)
        {
            if (string.IsNullOrEmpty(ErrorMessage)) this.ClearErrorFlags();
            this.Result.Set(ErrorClass.E6, ErrorMessage);
            throw new Exception(this.Result.ErrorMessage);
        }
        protected void CatchException(Exception ex)
        {
            this.Result.Set(this.Result.EClass == ErrorClass.OK ? ErrorClass.E6 : this.Result.EClass, this.FormatErrMsg2(null, ex));
        }
        protected void CatchException(ErrorClass eclass, string err)
        {
            this.Result.Set(eclass, err);
        }
        protected void CatchAndPromptErr(Exception ex)
        {
            this.CatchException(ex);
            //log error
        }
        protected void ClearErrorFlags()
        {
            this.Result.Reset();
        }
        protected void ThrowError(ErrorClass EClass, string ErrorMessage)
        {
            this.Result.Set(EClass, ErrorMessage);
            throw new Exception(ErrorMessage);
        }
        private void ThrowError(ErrorResult Result)
        {
            this.Result.Set(Result);
            throw new Exception(Result.ErrorMessage);
        }
        protected void CheckAndThrowIfError(string Result)
        {
            if (string.IsNullOrEmpty(Result))
            {
                this.ClearErrorFlags();
                return;
            }
            this.Result.Set(ErrorClass.E6, Result);
            if (this.Result.EClass != ErrorClass.OK) this.ThrowError(this.Result);
            else this.ClearErrorFlags();
        }
        protected void CheckAndThrowIfError(ErrorResult Result)
        {
            if (Result == null)
            {
                this.ClearErrorFlags();
                return;
            }
            this.Result.Set(Result);
            if (this.Result.EClass != ErrorClass.OK) this.ThrowError(this.Result);
            else this.ClearErrorFlags();
        }
        protected void CheckAndThrowIfError(ErrorClass EClassIfFail, string ErrorMessage)
        {
            this.Result.Set(EClassIfFail, ErrorMessage);
            if (this.Result.ErrorMessage != string.Empty) this.ThrowError(this.Result);
            else this.ClearErrorFlags();
        }
        protected void CheckAndThrowIfError(Task<ErrorResult>[] tasks)
        {
            Task.WaitAll(tasks);
            foreach (var task in tasks)
            {
                this.CheckAndThrowIfError(task.Result);
            }
            this.ClearErrorFlags();
        }
        protected void CheckAndThrowIfError(ErrorClass EClass, Task<string>[] tasks)
        {
            Task.WaitAll(tasks);
            foreach (var task in tasks)
            {
                this.CheckAndThrowIfError(EClass, task.Result);
            }
            this.ClearErrorFlags();
        }
        #endregion
        #region Cloneable
        public object Clone() { return this.MemberwiseClone(); }
        #endregion
        #region Getname
        protected string GetName([CallerMemberName] string name = null)
        {
            return name;
        }
        #endregion
    }
    public class ErrorResult
    {
        public ErrorClass EClass { get; set; }
        public string ErrorMessage { get; set; }
        public void Reset()
        {
            this.EClass = ErrorClass.OK;
            this.ErrorMessage = string.Empty;
        }
        public ErrorResult()
        {
            this.EClass = ErrorClass.OK;
            this.ErrorMessage = string.Empty;
        }
        public void Set(ErrorClass EClass, string ErrorMessage)
        {
            this.EClass = EClass;
            this.ErrorMessage = ErrorMessage;
        }
        public void Set(ErrorResult Source)
        {
            this.EClass = Source.EClass;
            this.ErrorMessage = Source.ErrorMessage;
        }
        public bool IsStoppable => (int)this.EClass >= (int)ErrorClass.E4;
    }
    [Serializable]
    public enum ErrorClass
    {
        OK,
        E1,//a note to user, no need action, proceeding to work. not an error
        E2,//an error has occur, no need action, proceeding to work on same dut
        E3,//an error has occur, no need action, skip to next dut
        E4,//an error has occur, need human intervention to check settings, machine stop
        E5,//an error has occur, need human intervention to check hardware, machine stop
        E6,//an error has occur, unclassified error, machine stop
        F,//speak to manufacturer
    }

    public class FileStreamHandler : BaseUtility
    {
        FileStream File;
        public ErrorResult GetConfigStream(string src)
        {
            try
            {
                if (File == null)
                {
                    var path = Path.GetDirectoryName(src);
                    if (Directory.Exists(path) == false)
                        Directory.CreateDirectory(path);
                    File = new FileStream(src, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
                }
            }
            catch (Exception ex)
            {
                this.FormatErrMsg(null, ex);
            }
            return this.Result;
        }
        public ErrorResult SaveStream(byte[] data)
        {
            try
            {
                if (this.File == null) return this.Result;
                this.File.Position = 0;
                this.File.Write(data, 0, data.Length);
                this.File.Flush();
                this.File.SetLength(data.Length);
            }
            catch (Exception ex)
            {
                this.FormatErrMsg(null, ex);
            }
            return this.Result;
        }
        public ErrorResult CloseStream()
        {
            try
            {
                this.File?.Close();
                this.File = null;
            }
            catch (Exception ex)
            {
                this.FormatErrMsg(null, ex);
            }
            return this.Result;
        }
    }

    public class Util
    {
        public static void LoadAssemblies(string src)
        {
            var files = Directory.GetFiles(src, "HiPA.*.dll");

            foreach (var file in files)
                Assembly.LoadFile(file);
        }

        public static Dictionary<string, string> GetAttributesandName<T>()
        {
            PropertyInfo[] props = typeof(T).GetProperties();
            var Out = new Dictionary<string, string>();
            foreach (PropertyInfo prop in props)
            {
                object[] attrs = prop.GetCustomAttributes(true);
                foreach (object attr in attrs)
                {
                    if (!(attr is TextAttribute)) continue;
                    if (attr is null) continue;
                    var authAttr = attr as TextAttribute;
                    Out[authAttr.Attribute] = prop.Name;
                }
            }
            return Out;
        }

        public static Task<(ErrorResult Error, List<T> Result)> GetIssuesList<T>(string FileName)
        {
            return Task.Run(() =>
            {
                var Issues = new List<T>();
                var Err = new ErrorResult();
                var ColumnIdx = new Dictionary<string, int?>();
                try
                {
                    ExcelPackage.License.SetNonCommercialPersonal("");
                    var Attr = Util.GetAttributesandName<T>();
                    using (ExcelPackage xlPackage = new ExcelPackage(new FileInfo(FileName)))
                    {

                        var myWorksheet = xlPackage.Workbook.Worksheets.First(); //select sheet here
                        var totalRows = myWorksheet.Dimension.End.Row;
                        var totalColumns = myWorksheet.Dimension.End.Column;
                        int idx = 0;
                        Issues.Clear();
                        var sb = new StringBuilder(); //this is your data
                        for (int rowNum = 1; rowNum < totalRows; rowNum++) //select starting row here
                        {
                            var Cols = myWorksheet.Cells[rowNum, 1, rowNum, totalColumns].Select(c => c.Value == null ? string.Empty : c.Value.ToString()).ToArray();
                            var H_Cols = myWorksheet.Cells[rowNum, 1, rowNum, totalColumns].Select(c => c.Hyperlink == null ? string.Empty : c.Hyperlink.ToString()).ToArray();
                            if (Cols.Length < 4) break;
                            if (rowNum == 1)
                            {
                                foreach (var title in Cols)
                                {
                                    if (Attr.ContainsKey(title))
                                        ColumnIdx[title] = Cols.ToList().IndexOf(title);
                                }
                                foreach (var attribute in Attr.Keys)
                                {
                                    if (ColumnIdx.ContainsKey(attribute)) continue;
                                    ColumnIdx[attribute] = null;
                                }
                            }
                            else
                            {
                                var Issue = (T)Activator.CreateInstance(typeof(T));
                                foreach (var col in ColumnIdx.Keys)
                                {
                                    if (ColumnIdx[col] == null) continue;
                                    if (string.Equals(col, "Linked Issues")) continue;
                                    PropertyInfo propertyInfo = Issue.GetType().GetProperty(Attr[col]);
                                    if (propertyInfo.PropertyType.Name == "String")
                                    {
                                        propertyInfo.SetValue(Issue, Cols[(int)ColumnIdx[col]], null);
                                    }
                                    else if (propertyInfo.PropertyType.Name == "sEntry")
                                    {
                                        var content = Cols[(int)ColumnIdx[col]];
                                        var ticketname = string.Empty;

                                        if (string.Equals(col, "Key"))
                                        {
                                            ticketname = content.Substring(0, content.Length);
                                        }
                                        else if (string.Equals(col, "Summary"))
                                        {
                                            var linkedissues = Cols[(int)ColumnIdx["Linked Issues"]];
                                            var issues = linkedissues.Split(',');
                                            var linkedtsgsds = issues.Where(x => x.Contains("TSGSD")).ToList();
                                            linkedtsgsds.ForEach(x => x.Replace(" ", string.Empty));
                                            var summary = Cols[(int)ColumnIdx["Summary"]];
                                            var tsgsdfromsummary = summary.Split(':', '(').First().Replace(" ", string.Empty);
                                            if (linkedtsgsds.Contains(tsgsdfromsummary))
                                                ticketname = tsgsdfromsummary;
                                            else if (tsgsdfromsummary.Contains("TSGSD"))
                                            {
                                                var splittsgsd = tsgsdfromsummary.Split('-');
                                                if (splittsgsd.Length == 2)
                                                {
                                                    if (int.TryParse(splittsgsd[1], out int result))
                                                    {
                                                        ticketname = tsgsdfromsummary;
                                                    }
                                                }
                                                else
                                                {
                                                    ticketname = linkedtsgsds.LastOrDefault();
                                                }
                                            }
                                            else
                                            {
                                                ticketname = linkedtsgsds.LastOrDefault();
                                            }
                                            PropertyInfo propertyInfoTSG = Issue.GetType().GetProperty("TSGSD");
                                            propertyInfoTSG.SetValue(Issue, content, null);
                                        }
                                        if (string.IsNullOrEmpty(ticketname))
                                            continue;
                                        var link = new sEntry();
                                        link.Uri = (new Uri(new Uri("https://jira.besi.com/browse/"), ticketname)).ToString();
                                        link.Value = ticketname;
                                        propertyInfo.SetValue(Issue, link, null);
                                    }
                                    else if (propertyInfo.PropertyType.Name == "DateTime")
                                        propertyInfo.SetValue(Issue, DateTime.ParseExact(Cols[(int)ColumnIdx[col]], "dd-MM-yyyy", CultureInfo.CurrentCulture), null);
                                }
                                Issues.Add(Issue);
                            }
                            sb.Clear();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                return (Err, Issues);
            });
        }
    }

    public static class ReflectionTool
    {
        public static string GetEnumDescription(Enum value)
        {
            var ev = value.GetHashCode();
            var type = value.GetType();
            foreach (var v in GetEnumValueDesc(type))
                if (v.Value.CompareTo(ev) == 0)
                    return v.Desc;
            return value.ToString();
        }
        //public static string GetEnumDescription( Enum value )
        //{
        //	var fi = value.GetType().GetField( value.ToString() );
        //	var attributes = ( DescriptionAttribute[] )fi.GetCustomAttributes( typeof( DescriptionAttribute ), false );

        //	return ( attributes != null && attributes.Length > 0 ) ? 
        //		attributes[ 0 ].Description : 
        //		value.ToString();
        //}

        public static (int Address, T Attribute) GetEnumAttribute<T>(Enum point) where T : Attribute
        {
            var type = point.GetType();
            var str = point.ToString();
            foreach (var name in Enum.GetNames(type))
            {
                if (name == str)
                {
                    var value = Enum.Parse(type, name);
                    var v = Convert.ToInt32(value);
                    var field = type.GetField(name);
                    var fds = field.GetCustomAttributes<T>(true);
                    if (fds.Count() > 0)
                        return (v, fds.First());
                }
            }
            return (-1, null);
        }

        public static IEnumerable<string> GetEnumDescriptions(Type type)
        {
            foreach (var name in Enum.GetNames(type))
            {
                var field = type.GetField(name);
                var fds = field.GetCustomAttributes<DescriptionAttribute>(true);
                if (fds.Count() > 0)
                    yield return fds.First().Description;
            }
        }

        public static IEnumerable<(int Value, string Desc)> GetEnumValueDesc(Type type)
        {
            foreach (var name in Enum.GetNames(type))
            {
                var value = Enum.Parse(type, name);
                var v = Convert.ToInt32(value);

                var field = type.GetField(value.ToString());
                var fds = field.GetCustomAttributes<DescriptionAttribute>(true);
                if (fds.Count() > 0)
                    yield return (v, fds.First().Description);
            }
        }
        public static IEnumerable<(int Value, T Attribute)> GetEnumAttributes<T>(Type type) where T : Attribute
        {
            foreach (var name in Enum.GetNames(type))
            {
                var value = Enum.Parse(type, name);
                var v = Convert.ToInt32(value);
                var field = type.GetField(name);
                var fds = field.GetCustomAttributes<T>(true);
                if (fds.Count() > 0)
                    yield return (v, fds.First());
            }
        }

        public static void SetPropertyValue(object instance, string name, object value)
        {
            if (instance == null) return;
            var type = instance.GetType();
            var prop = type.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            prop?.SetValue(instance, value);
        }

        public static string GetPropertyName<T>(Expression<Func<T>> propertyExpression)
        {
            return (propertyExpression.Body as MemberExpression).Member.Name;
        }

        public static Dictionary<Type, List<Type>> QueryConfigurationTypes(IEnumerable<Type> validTypes)
        {
            var typePairs = new Dictionary<Type, List<Type>>();
            var something = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var valid in validTypes)
                {
                    if (assembly.IsDynamic == true) continue;

                    var pair = FetchTypes(assembly, valid);
                    if (pair.Any())
                    {
                        if (typePairs.TryGetValue(valid, out var list) == true)
                        {
                            list.AddRange(pair);
                        }
                        else
                        {
                            list = new List<Type>();
                            list.AddRange(pair);
                            typePairs.Add(valid, list);
                        }
                    }
                }
            }
            return typePairs;
        }

        public static Type GetEnumType(string enumName)
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                var type = assembly.GetType(enumName);
                if (type == null)
                    continue;
                if (type.IsEnum)
                    return type;
            }
            return null;
        }


        public static IEnumerable<Type> FetchTypes(Assembly assembly, Type validType)
        {
            foreach (var type in assembly.ExportedTypes)
            {
                if (validType.IsAssignableFrom(type) &&
                    type.IsInterface == false &&
                    type.IsAbstract == false)
                {
                    yield return type;
                }
            }
        }
        public static T GetPropertyValue<T>(object obj, string propName) { return (T)obj.GetType().GetProperty(propName).GetValue(obj, null); }
    }
    public static class XmlHelper
    {
        private static void XmlSerializeInternal(Stream stream, object o, Type[] extraTypes = null)
        {
            if (o == null) throw new ArgumentNullException("o");

            if (extraTypes == null) extraTypes = ExtraTypes;
            else AddExtraTypes(extraTypes);

            XmlSerializer serializer = new XmlSerializer(o.GetType(), extraTypes);

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.NewLineChars = "\r\n";
            settings.Encoding = Encoding.UTF8;
            settings.IndentChars = "    ";

            using (XmlWriter writer = XmlWriter.Create(stream, settings))
            {
                serializer.Serialize(writer, o);
                writer.Close();
            }
        }


        public static string XmlSerialize(object o, Type[] extraTypes = null)
        {
            if (extraTypes == null) extraTypes = ExtraTypes;
            else AddExtraTypes(extraTypes);

            using (MemoryStream stream = new MemoryStream())
            {
                XmlSerializeInternal(stream, o, extraTypes);

                stream.Position = 0;
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        public static void XmlSerializeToFile(object o, string path, Type[] extraTypes = null)
        {
            if (string.IsNullOrEmpty(path)) throw new ArgumentNullException($"The path is invalid");

            if (extraTypes == null) extraTypes = ExtraTypes;
            else AddExtraTypes(extraTypes);

            using (FileStream file = new FileStream(path, FileMode.Create, FileAccess.Write))
            {
                XmlSerializeInternal(file, o, extraTypes);
            }
        }

        public static T XmlDeserialize<T>(string s, Type[] extraTypes = null)
        {
            return (T)XmlDeserialize(typeof(T), s, extraTypes);
        }
        public static object XmlDeserialize(Type type, string s, Type[] extraTypes = null)
        {
            if (string.IsNullOrEmpty(s)) throw new ArgumentNullException("The stream is invalid");
            var encoding = Encoding.UTF8;

            if (extraTypes == null) extraTypes = ExtraTypes;
            else AddExtraTypes(extraTypes);

            XmlSerializer mySerializer = new XmlSerializer(type, extraTypes);
            using (MemoryStream ms = new MemoryStream(encoding.GetBytes(s)))
            {
                using (StreamReader sr = new StreamReader(ms, encoding))
                {
                    return mySerializer.Deserialize(sr);
                }
            }
        }

        public static T XmlDeserializeFromFile<T>(string path, Type[] extraTypes = null)
        {
            return (T)XmlDeserializeFromFile(typeof(T), path, extraTypes);
        }
        public static object XmlDeserializeFromFile(Type type, string path, Type[] extraTypes = null)
        {
            if (string.IsNullOrEmpty(path)) throw new ArgumentNullException($"The path is invalid");

            string xml = File.ReadAllText(path, Encoding.UTF8);
            return XmlDeserialize(type, xml, extraTypes);
        }



        static List<Type> s_extraTypes = new List<Type>();
        public static Type[] ExtraTypes => s_extraTypes.ToArray();

        public static void AddExtraTypes(Type[] extraTypes)
        {
            if (extraTypes == null) return;
            foreach (var type in extraTypes)
            {
                if (s_extraTypes.Contains(type) == false)
                    s_extraTypes.Add(type);
            }
        }
    }

    public class XmlDictionary<TKey, TValue>
        : Dictionary<TKey, TValue>
        , IXmlSerializable
    {
        public XmlDictionary()
        {
        }
        public XmlDictionary(IDictionary<TKey, TValue> dictionary)
            : base(dictionary)
        {
        }

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            reader.Read();
            XmlSerializer KeySerializer = new XmlSerializer(typeof(TKey));
            XmlSerializer ValueSerializer = new XmlSerializer(typeof(TValue), XmlHelper.ExtraTypes);

            //name = reader.Name;
            //reader.ReadStartElement( name );
            while (reader.NodeType != XmlNodeType.EndElement)
            {
                if (reader.IsEmptyElement == true) break;

                reader.ReadStartElement("element");   // <element>
                reader.ReadStartElement("key");       //	<key>
                TKey tk = (TKey)KeySerializer.Deserialize(reader);
                reader.ReadEndElement();                //  </key>
                reader.ReadStartElement("value");     //  <value>
                TValue vl = (TValue)ValueSerializer.Deserialize(reader);
                reader.ReadEndElement();                //  </value>
                reader.ReadEndElement();                // <element>

                this.Add(tk, vl);
                reader.MoveToContent();
            }
            //reader.ReadEndElement();
            //reader.ReadEndElement();
        }
        public void WriteXml(XmlWriter writer)
        {
            XmlSerializer KeySerializer = new XmlSerializer(typeof(TKey));
            XmlSerializer ValueSerializer = new XmlSerializer(typeof(TValue), XmlHelper.ExtraTypes);

            //write.WriteAttributeString( "name", name );
            //write.WriteStartElement( name );
            foreach (KeyValuePair<TKey, TValue> kv in this)
            {
                writer.WriteStartElement("element");
                writer.WriteStartElement("key");
                KeySerializer.Serialize(writer, kv.Key);
                writer.WriteEndElement();
                writer.WriteStartElement("value");
                ValueSerializer.Serialize(writer, kv.Value);
                writer.WriteEndElement();
                writer.WriteEndElement();
            }
            //write.WriteEndElement();
        }

        public void ReadXmlGeneric(XmlReader reader)
        {
            reader.Read();
            XmlSerializer KeySerializer = new XmlSerializer(typeof(TKey));
            XmlSerializer ValueSerializer = new XmlSerializer(typeof(TValue));

            //name = reader.Name;
            //reader.ReadStartElement( name );
            reader.ReadStartElement("element");   // <element>
            while (reader.NodeType != XmlNodeType.EndElement)
            {
                if (reader.IsEmptyElement == true) break;

                reader.ReadStartElement("key");       //	<key>
                TKey tk = (TKey)KeySerializer.Deserialize(reader);
                reader.ReadEndElement();                //  </key>
                reader.ReadStartElement("value");     //  <value>
                TValue vl = (TValue)ValueSerializer.Deserialize(reader);
                reader.ReadEndElement();                //  </value>

                this.Add(tk, vl);
                reader.MoveToContent();
            }

            reader.ReadEndElement();                // <element>
                                                    //reader.ReadEndElement();
                                                    //reader.ReadEndElement();
        }

        public void WriteXmlGeneric(XmlWriter writer)
        {
            XmlSerializer KeySerializer = new XmlSerializer(typeof(TKey));
            XmlSerializer ValueSerializer = new XmlSerializer(typeof(TValue));

            //write.WriteAttributeString( "name", name );
            //writer.WriteStartElement( "Test" );
            writer.WriteStartElement("element");
            foreach (KeyValuePair<TKey, TValue> kv in this)
            {
                writer.WriteStartElement("key");
                KeySerializer.Serialize(writer, kv.Key);
                writer.WriteEndElement();
                writer.WriteStartElement("value");
                ValueSerializer.Serialize(writer, kv.Value);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }


        #region Read/Write XML for at least 2 level nesting objects
        public void ReadXml_2LevelNesting(System.Xml.XmlReader reader)
        {
            XmlSerializer keySerializer = new XmlSerializer(typeof(TKey));
            XmlSerializer valueSerializer = new XmlSerializer(typeof(TValue));

            bool wasEmpty = reader.IsEmptyElement;
            reader.Read();
            if (wasEmpty) return;

            reader.ReadStartElement("item");                                          //#1
            while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
            {
                reader.ReadStartElement("key");                                       //#2
                TKey key = (TKey)keySerializer.Deserialize(reader);                 //#3
                reader.ReadEndElement();                                                //#4

                reader.ReadStartElement("value");                                     //#5
                TValue value = (TValue)valueSerializer.Deserialize(reader);         //#6
                reader.ReadEndElement();                                                //#7

                this.Add(key, value);

                reader.ReadEndElement();                                                //#8
                reader.MoveToContent();                                                 //#9
            }
            reader.ReadEndElement();                                                    //#10end
        }

        public void WriteXml_2LevelNesting(System.Xml.XmlWriter writer)
        {
            XmlSerializer keySerializer = new XmlSerializer(typeof(TKey));
            XmlSerializer valueSerializer = new XmlSerializer(typeof(TValue));

            writer.WriteStartElement("item");             //#1
            foreach (TKey key in this.Keys)
            {
                writer.WriteStartElement("key");          //#2
                keySerializer.Serialize(writer, key);     //#3
                writer.WriteEndElement();                   //#4

                writer.WriteStartElement("value");        //#5
                TValue value = this[key];
                valueSerializer.Serialize(writer, value); //#6
                writer.WriteEndElement();                   //#7
            }
            writer.WriteEndElement();                       //#8
        }
        #endregion
    }
    public class EnumDescriptionConverter : IValueConverter
    {
        private string GetEnumDescription(Enum enumObj)
        {
            FieldInfo fieldInfo = enumObj.GetType().GetField(enumObj.ToString());

            object[] attribArray = fieldInfo.GetCustomAttributes(false);

            if (attribArray.Length == 0)
            {
                return enumObj.ToString();
            }
            else
            {
                DescriptionAttribute attrib = attribArray[0] as DescriptionAttribute;
                return attrib.Description;
            }
        }

        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Enum myEnum = (Enum)value;
            string description = GetEnumDescription(myEnum);
            return description;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.Empty;
        }
    }
    public class RecipeNoticeHandler
    {
        public static event PropertyChangedEventHandler StaticPropertyChanged;
        private static bool bRecipeChanged = false;
        public static bool RecipeChanged
        {
            get => bRecipeChanged;
            set
            {
                bRecipeChanged = value;
                StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs("RecipeChanged"));
            }
        }
    }
    [Serializable]
    public abstract class Configuration
           : RecipeBaseUtility
    {
        public abstract string Name { get; set; }
        public string Location { get; set; } = "";
        public abstract Type InstrumentType { get; }

        protected bool b_Enable = true;
        public bool Enable
        {
            get => b_Enable;
            set => Set(ref b_Enable, value, "Enable");
        }
        public List<string> Children { get; set; }

        public virtual void CheckDefaultValue()
        {
            _CheckDefaultValue(this);
        }

        public static void _CheckDefaultValue(object instance)
        {
            var type = instance.GetType();
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            var defaultField = type.GetField("_DEFAULT", BindingFlags.Static | BindingFlags.NonPublic);
            var defaultObject = defaultField?.GetValue(instance);
            if (defaultObject == null) return;

            foreach (var field in fields)
            {
                var value = field.GetValue(instance);
                if (value is null)
                {
                    var f = defaultField.DeclaringType.GetField(field.Name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                    value = f.GetValue(defaultObject);

                    var newObj = ObjectCopy.Copy(value);

                    field.SetValue(instance, newObj);
                }
            }
        }
    }
    public class sEntry : BaseUtility
    {
        public new string Value
        {
            get => GetValue(() => Value);
            set => SetValue(() => Value, value);
        }
        public string Uri
        {
            get => GetValue(() => Uri);
            set => SetValue(() => Uri, value);
        }
        public sEntry() { }
    }
    public enum eBool
    {
        False,
        True,
    }
}
