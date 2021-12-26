using System;
using System.Globalization;
using Newtonsoft.Json;

namespace Core.Domain.Utilities {
    /// <summary>
    /// Json 相關工具
    /// </summary>
    public static class JsonUtility {
        /// <summary>
        /// 反序列化字串成物件
        /// </summary>
        /// <typeparam name="T">物件型別</typeparam>
        /// <param name="str">字串</param>
        /// <returns>物件</returns>
        public static T Deserialize<T>(string str){
            T result = JsonConvert.DeserializeObject<T>(str);
            return result;
        }

        /// <summary>
        /// 序列化物件為字串
        /// </summary>
        /// <param name="obj">物件</param>
        /// <param name="isIndented">是否縮排</param>
        /// <returns>字串</returns>
        public static string Serialize(object obj, bool isIndented = false, bool isIgnorezNullValue = false) {
            string result;


            if(isIndented){
                if(isIgnorezNullValue){
                    JsonSerializerSettings settings = new JsonSerializerSettings { 
                        NullValueHandling = NullValueHandling.Ignore
                    };
                    result = JsonConvert.SerializeObject(obj, Formatting.Indented, settings);
                } else {
                    result = JsonConvert.SerializeObject(obj, Formatting.Indented);
                }
            } else {
                if(isIgnorezNullValue){
                    JsonSerializerSettings settings = new JsonSerializerSettings { 
                        NullValueHandling = NullValueHandling.Ignore
                    };
                    result = JsonConvert.SerializeObject(obj, settings);
                } else {
                    result = JsonConvert.SerializeObject(obj);
                }
            }
            
            return result;
        }
    }
}