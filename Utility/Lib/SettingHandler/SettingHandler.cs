using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Utility.Lib.SettingHandler
{
    public class SettingHandler<T> : BaseUtility
        where T : aSaveable
    {
        FileStreamHandler FStream = new FileStreamHandler();
        public event EventHandler SettingLoaded;
        public T Get => this.Data;
        private T Data { get; set; }
        private string XmlLocation
        {
            get => this.GetValue(() => this.XmlLocation);
            set => this.SetValue(() => this.XmlLocation, value);
        }
        public ErrorResult Save()
        {
            this.ClearErrorFlags();
            try
            {
                this.FStream.GetConfigStream(this.XmlLocation);
                if (this.Data == null) this.Data = (T)Activator.CreateInstance(typeof(T));
                var stream = XmlHelper.XmlSerialize(this.Data, this._xmlExtraTypes);
                var data = Encoding.UTF8.GetBytes(stream);
                this.CheckAndThrowIfError(this.FStream.SaveStream(data));
            }
            catch (Exception ex)
            {
                this.FormatErrMsg(string.Empty, ex);
                MessageBox.Show(ex.Message);
            }
            finally
            {
                this.FStream.CloseStream();
            }
            return this.Result;
        }
        public string Load()
        {
            var result = string.Empty;
            try
            {

                if ((result = this.LoadConfigurationTypes()) != String.Empty) throw new Exception(result);

                if (File.Exists(this.XmlLocation))
                {
                    try
                    {
                        this.Data = (T)XmlHelper.XmlDeserializeFromFile(typeof(T), this.XmlLocation, this._xmlExtraTypes);
                    }
                    catch (Exception ex)
                    {
                        this.Data = null;
                        //MessageBox.Show(ex.Message);
                    }
                }

                if (this.Data == null) this.Data = (T)Activator.CreateInstance(typeof(T));
                //this.FStream.GetConfigStream(this.XmlLocation);
                this.SettingLoaded?.Invoke(this, null);
            }
            catch (Exception ex)
            {
                result = this.FormatErrMsg(string.Empty, ex);
                MessageBox.Show(ex.Message);
            }
            return result;
        }

        #region Check Configuration Types 
        Type[] _xmlExtraTypes = null;
        string LoadConfigurationTypes()
        {
            var result = string.Empty;
            try
            {
                var type = typeof(T);

                Util.LoadAssemblies(this.StartupPath);
                if (ReflectionTool.QueryConfigurationTypes(new Type[] { type }).TryGetValue(type, out var pair) == true)
                {
                    this._xmlExtraTypes = pair.ToArray();
                }
            }
            catch (Exception ex)
            {
                result = $",Constructor:,LoadConfigurationTypes:,{ex.Message}";
            }
            return result;
        }
        #endregion

        string StartupPath = AppDomain.CurrentDomain.BaseDirectory;

        public SettingHandler(string Path)
        {
            this.XmlLocation = Path;
        }
        public SettingHandler()
        {
        }
        public void SetPath(string path)
        {
            this.XmlLocation = path;
        }
        public void SetPathnLoad(string path)
        {
            this.SetPath(path);
            this.Load();
        }
        public void SetPathnSave(string path)
        {
            this.SetPath(path);
            this.Save();
        }
    }
}
