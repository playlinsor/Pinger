using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlaylinsorLib
{
    /// <summary>
    /// Класс локализации строк и работа с языками
    /// </summary>
    public class Localization
    {
        SortedList<string, string> LanguageLabels = new SortedList<string, string>();
        public enum Languages {Russian=1,English=2}
        Languages CurrentLang = Languages.Russian;

        public Localization(Languages lang)
        {
            LoadLang(lang);
        }

        /// <summary>
        /// Вернуть текущий язык
        /// </summary>
        /// <returns></returns>
        public Languages GetCurrentLang()
        {
            return CurrentLang;
        }

        /// <summary>
        /// Загрузить языковую локаль
        /// </summary>
        /// <param name="lang"></param>
        public void LoadLang(Languages lang)
        {
            string LangText = "";
            CurrentLang = lang;

            switch (lang)
            {
                case Languages.Russian: LangText = Pinger.Properties.Resources.Ru  ;break;   
                case Languages.English: LangText = Pinger.Properties.Resources.Eng ;break;
                default: LangText = Pinger.Properties.Resources.Eng; break;
            }

            string[] labels = LangText.Split(';');

            LanguageLabels.Clear();

            foreach (string item in labels)
            {
                
                if (item.IndexOf('=')!=-1 && item[0]!='#')
                {
                    string Key = (item.Substring(0, item.IndexOf('='))).Trim();

                    string Value = (item.Substring(item.IndexOf('='), item.Length - item.IndexOf('='))).Split('"')[1];
                    
                    if (!LanguageLabels.ContainsKey(Key))
                        
                        LanguageLabels.Add(Key,Value );
                }
            }
        }

        /// <summary>
        /// .toString() преобразование системных надписей.
        /// </summary>
        /// <returns></returns>
        public IFormatProvider GetCultureFormat()
        {
            return new System.Globalization.CultureInfo(GetString("CultureInfo"));
        }

        #region Вернуть строку по ID
        public string GetString(string lang)
        {
            string ReturnText = "";
            try
            {
                ReturnText = LanguageLabels[lang];
                ReturnText = ReturnText.Replace("%n", "\n");
            }
            catch 
            {
                ReturnText = "STRING:"+lang;
            }

            return ReturnText;
        }

        public string GetString(string lang, Object OneParam)
        {
            return GetString(lang).Replace("%s1", OneParam.ToString());
        }

        public string GetString(string lang, Object OneParam, Object TwoParam)
        {
           return GetString(lang,OneParam).Replace("%s2", TwoParam.ToString());
            
        }

        public string GetString(string lang, Object OneParam, Object TwoParam, Object TreeParam)
        {
            return GetString(lang, OneParam, TwoParam).Replace("%s3", TreeParam.ToString());      
        }

        public string GetString(string lang, Object OneParam, Object TwoParam, Object TreeParam, Object FourParam)
        {
            return GetString(lang, OneParam.ToString(), TwoParam.ToString(),TreeParam.ToString()).Replace("%s4", FourParam.ToString());
        }

        #endregion
    }
}
