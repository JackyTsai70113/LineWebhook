using System;
using System.Collections.Generic;
using BL.Services.Cache;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Website.Models;

namespace Website.Controllers {
    public class RedisController : ControllerBase {
        private readonly ICacheService _redisCacheService;

        public RedisController(ICacheService cacheService) {
            _redisCacheService = cacheService;
        }

        /// <summary>
        /// 取得所有的KeyValues
        /// </summary>
        /// <returns>KeyValues</returns>
        [HttpGet]
        [Route("redis/keyValues")]
        public BaseResponseModel<List<KeyValue>, string> GetAllKeyValues() {
            BaseResponseModel<List<KeyValue>, string> responseModel;
            List<string> keys = new List<string>();
            try {
                keys = _redisCacheService.GetKeys("");
                List<string> noExistKeys = new List<string>();
                List<KeyValue> keyValues = new List<KeyValue>();
                foreach (string key in keys) {
                    if (!_redisCacheService.ExistKeyValue(key)) {
                        noExistKeys.Add(key);
                    }

                    string value = _redisCacheService.Get<string>(key);
                    keyValues.Add(new KeyValue() {
                        key = key,
                        value = value
                    });
                }
                responseModel = new BaseResponseModel<List<KeyValue>, string>() {
                    isSuccess = true,
                    data = keyValues,
                    error = $"noExistKey: {JsonConvert.SerializeObject(noExistKeys)}"
                };
                return responseModel;
            } catch (Exception ex) {
                responseModel = new BaseResponseModel<List<KeyValue>, string>() {
                    isSuccess = false,
                    data = null,
                    error = $"keys: {JsonConvert.SerializeObject(keys)}, ex: {ex}"
                };
                return responseModel;
            }
        }

        /// <summary>
        /// 取得KeyValue By <paramref name="key"/>
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>KeyValue</returns>
        [HttpGet]
        [Route("redis/keyValues/{key}")]
        public BaseResponseModel<KeyValue, string> GetKeyValueByKey(string key) {
            BaseResponseModel<KeyValue, string> responseModel;
            try {
                if (!_redisCacheService.ExistKeyValue(key)) {
                    responseModel = new BaseResponseModel<KeyValue, string>() {
                        isSuccess = false,
                        data = null,
                        error = "key doesn't exist."
                    };
                    return responseModel;
                }
                string value = _redisCacheService.Get<string>(key);
                responseModel = new BaseResponseModel<KeyValue, string>() {
                    isSuccess = true,
                    data = new KeyValue() {
                        key = key,
                        value = value
                    },
                    error = null
                };
                return responseModel;
            } catch (Exception ex) {
                responseModel = new BaseResponseModel<KeyValue, string>() {
                    isSuccess = false,
                    data = null,
                    error = ex.ToString()
                };
                return responseModel;
            }
        }
        
        /// <summary>
        /// 取得KeyValues By <paramref name="keys"/>
        /// </summary>
        /// <param name="keys">keys</param>
        /// <returns>KeyValues</returns>
        [HttpPost]
        [Route("redis/GetKeyValuesByKeys")]
        public BaseResponseModel<List<KeyValue>, string> GetKeyValuesByKeys([FromBody] List<string> keys) {
            BaseResponseModel<List<KeyValue>, string> responseModel;
            try {
                List<string> noExistKeys = new List<string>();
                List<KeyValue> keyValues = new List<KeyValue>();
                foreach (string key in keys) {
                    if (!_redisCacheService.ExistKeyValue(key)) {
                        noExistKeys.Add(key);
                    } else{
                        string value = _redisCacheService.Get<string>(key);
                        keyValues.Add(new KeyValue() {
                            key = key,
                            value = value
                        });
                    }
                }
                responseModel = new BaseResponseModel<List<KeyValue>, string>() {
                    isSuccess = true,
                    data = keyValues,
                    error = $"noExistKeys: {JsonConvert.SerializeObject(noExistKeys)}"
                };
                return responseModel;
            } catch (Exception ex) {
                responseModel = new BaseResponseModel<List<KeyValue>, string>() {
                    isSuccess = false,
                    data = null,
                    error = $"keys: {JsonConvert.SerializeObject(keys)}, ex: {ex}"
                };
                return responseModel;
            }
        }

        /// <summary>
        /// 創建KeyValue
        /// </summary>
        /// <param name="keyValue">KeyValue</param>
        /// <returns>keyValue</returns>
        [HttpPost]
        [Route("redis/keyValues")]
        public BaseResponseModel<KeyValue, string> CreateKeyValue([FromBody] KeyValue keyValue) {
            BaseResponseModel<KeyValue, string> responseModel;
            bool setResult;
            try {
                if (_redisCacheService.ExistKeyValue(keyValue.key)) {
                    responseModel = new BaseResponseModel<KeyValue, string>() {
                        isSuccess = false,
                        data = null,
                        error = "key already exists."
                    };
                    return responseModel;
                }
                setResult = _redisCacheService.Set(keyValue.key, keyValue.value);
            } catch (Exception ex) {
                responseModel = new BaseResponseModel<KeyValue, string>() {
                    isSuccess = false,
                    data = null,
                    error = ex.ToString()
                };
                return responseModel;
            }

            if (setResult) {
                responseModel = new BaseResponseModel<KeyValue, string>() {
                    isSuccess = true,
                    data = new KeyValue() {
                        key = keyValue.key,
                        value = keyValue.value
                    },
                    error = null
                };
            } else {
                responseModel = new BaseResponseModel<KeyValue, string>() {
                    isSuccess = false,
                    data = null,
                    error = "Unknown error."
                };
            }
            return responseModel;
        }

        /// <summary>
        /// 創建或更新 <paramref name="keyValue"/>
        /// </summary>
        /// <param name="keyValue">keyValue</param>
        /// <returns>keyValue</returns>
        [HttpPut]
        [Route("redis/keyValues")]
        public BaseResponseModel<KeyValue, string> CreateOrUpdateKeyValue([FromBody] KeyValue keyValue) {
            BaseResponseModel<KeyValue, string> responseModel;
            bool setResult;
            try {
                setResult = _redisCacheService.Set(keyValue.key, keyValue.value);
            } catch (Exception ex) {
                responseModel = new BaseResponseModel<KeyValue, string>() {
                    isSuccess = false,
                    data = null,
                    error = ex.ToString()
                };
                return responseModel;
            }

            if (setResult) {
                responseModel = new BaseResponseModel<KeyValue, string>() {
                    isSuccess = true,
                    data = new KeyValue() {
                        key = keyValue.key,
                        value = keyValue.value
                    },
                    error = null
                };
            } else {
                responseModel = new BaseResponseModel<KeyValue, string>() {
                    isSuccess = false,
                    data = null,
                    error = "Unknown error."
                };
            }
            return responseModel;
        }

        /// <summary>
        /// 刪除KeyValue By <paramref name="key"/>
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>結果</returns>
        [HttpDelete]
        [Route("redis/keyValues/{key}")]
        public BaseResponseModel<string, string> DeleteKeyValueByKey(string key){
            BaseResponseModel<string, string> responseModel;
            bool deleteResult;
            try {
                if (!_redisCacheService.ExistKeyValue(key)) {
                    responseModel = new BaseResponseModel<string, string>() {
                        isSuccess = false,
                        data = null,
                        error = "key doesn't exist."
                    };
                    return responseModel;
                }
                deleteResult = _redisCacheService.Delete(key);
            } catch (Exception ex) {
                responseModel = new BaseResponseModel<string, string>() {
                    isSuccess = false,
                    data = null,
                    error = ex.ToString()
                };
                return responseModel;
            }

            if (deleteResult) {
                responseModel = new BaseResponseModel<string, string>() {
                    isSuccess = true,
                    data = "Delete keyValue success.",
                    error = null
                };
            } else {
                responseModel = new BaseResponseModel<string, string>() {
                    isSuccess = false,
                    data = null,
                    error = "Unknown error."
                };
            }
            return responseModel;
        }

        /// <summary>
        /// 取得Keys By <paramref name="pattern"/>
        /// </summary>
        /// <param name="pattern">pattern</param>
        /// <returns>Keys</returns>
        [HttpGet]
        [Route("redis/GetKeysByPattern")]
        public BaseResponseModel<List<string>, string> GetKeysByPattern(string pattern) {
            BaseResponseModel<List<string>, string> responseModel;
            try
            {
                List<string> keys = _redisCacheService.GetKeys(pattern);
                responseModel = new BaseResponseModel<List<string>, string>() {
                    isSuccess = true,
                    data = keys,
                    error = null
                };
                return responseModel;
            }
            catch (Exception ex)
            {
                responseModel = new BaseResponseModel<List<string>, string>() {
                    isSuccess = false,
                    data = null,
                    error = ex.ToString()
                };
                return responseModel;
            }
        }
    }

    /// <summary>
    /// KeyValue
    /// </summary>
    public class KeyValue{
        /// <summary>
        /// key
        /// </summary>
        public string key { get; set; }
        /// <summary>
        /// value
        /// </summary>
        public string value { get; set; }
    }
}