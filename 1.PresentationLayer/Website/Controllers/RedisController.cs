using System;
using System.Collections.Generic;
using System.Text.Json;
using BL.Service.Cache;
using Microsoft.AspNetCore.Mvc;
using Website.Models;

namespace Website.Controllers
{
    public class RedisController : ControllerBase
    {
        private readonly ICacheService RedisCacheService;

        public RedisController(ICacheService cacheService)
        {
            RedisCacheService = cacheService;
        }

        /// <summary>
        /// 取得所有的KeyValues
        /// </summary>
        /// <returns>KeyValues</returns>
        [HttpGet]
        [Route("redis/keyValues")]
        public BaseResponseModel<List<KeyValue>, string> GetAllKeyValues()
        {
            BaseResponseModel<List<KeyValue>, string> responseModel;
            List<string> keys = new();
            try
            {
                keys = RedisCacheService.GetKeys("");
                List<string> noExistKeys = new();
                List<KeyValue> keyValues = new();
                foreach (string key in keys)
                {
                    if (!RedisCacheService.ExistKeyValue(key))
                    {
                        noExistKeys.Add(key);
                    }

                    string value = RedisCacheService.Get<string>(key);
                    keyValues.Add(new KeyValue()
                    {
                        Key = key,
                        Value = value
                    });
                }
                responseModel = new BaseResponseModel<List<KeyValue>, string>()
                {
                    IsSuccess = true,
                    Data = keyValues,
                    Error = $"noExistKey: {JsonSerializer.Serialize(noExistKeys)}"
                };
                return responseModel;
            }
            catch (Exception ex)
            {
                responseModel = new BaseResponseModel<List<KeyValue>, string>()
                {
                    IsSuccess = false,
                    Data = null,
                    Error = $"keys: {JsonSerializer.Serialize(keys)}, ex: {ex}"
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
        public BaseResponseModel<KeyValue, string> GetKeyValueByKey(string key)
        {
            BaseResponseModel<KeyValue, string> responseModel;
            try
            {
                if (!RedisCacheService.ExistKeyValue(key))
                {
                    responseModel = new BaseResponseModel<KeyValue, string>()
                    {
                        IsSuccess = false,
                        Data = null,
                        Error = "key doesn't exist."
                    };
                    return responseModel;
                }
                string value = RedisCacheService.Get<string>(key);
                responseModel = new BaseResponseModel<KeyValue, string>()
                {
                    IsSuccess = true,
                    Data = new KeyValue()
                    {
                        Key = key,
                        Value = value
                    },
                    Error = null
                };
                return responseModel;
            }
            catch (Exception ex)
            {
                responseModel = new BaseResponseModel<KeyValue, string>()
                {
                    IsSuccess = false,
                    Data = null,
                    Error = ex.ToString()
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
        public BaseResponseModel<List<KeyValue>, string> GetKeyValuesByKeys([FromBody] List<string> keys)
        {
            BaseResponseModel<List<KeyValue>, string> responseModel;
            try
            {
                List<string> noExistKeys = new();
                List<KeyValue> keyValues = new();
                foreach (string key in keys)
                {
                    if (!RedisCacheService.ExistKeyValue(key))
                    {
                        noExistKeys.Add(key);
                    }
                    else
                    {
                        string value = RedisCacheService.Get<string>(key);
                        keyValues.Add(new KeyValue()
                        {
                            Key = key,
                            Value = value
                        });
                    }
                }
                responseModel = new BaseResponseModel<List<KeyValue>, string>()
                {
                    IsSuccess = true,
                    Data = keyValues,
                    Error = $"noExistKeys: {JsonSerializer.Serialize(noExistKeys)}"
                };
                return responseModel;
            }
            catch (Exception ex)
            {
                responseModel = new BaseResponseModel<List<KeyValue>, string>()
                {
                    IsSuccess = false,
                    Data = null,
                    Error = $"keys: {JsonSerializer.Serialize(keys)}, ex: {ex}"
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
        public BaseResponseModel<KeyValue, string> CreateKeyValue([FromBody] KeyValue keyValue)
        {
            BaseResponseModel<KeyValue, string> responseModel;
            bool setResult;
            try
            {
                if (RedisCacheService.ExistKeyValue(keyValue.Key))
                {
                    responseModel = new BaseResponseModel<KeyValue, string>()
                    {
                        IsSuccess = false,
                        Data = null,
                        Error = "key already exists."
                    };
                    return responseModel;
                }
                setResult = RedisCacheService.Set(keyValue.Key, keyValue.Value);
            }
            catch (Exception ex)
            {
                responseModel = new BaseResponseModel<KeyValue, string>()
                {
                    IsSuccess = false,
                    Data = null,
                    Error = ex.ToString()
                };
                return responseModel;
            }

            if (setResult)
            {
                responseModel = new BaseResponseModel<KeyValue, string>()
                {
                    IsSuccess = true,
                    Data = new KeyValue()
                    {
                        Key = keyValue.Key,
                        Value = keyValue.Value
                    },
                    Error = null
                };
            }
            else
            {
                responseModel = new BaseResponseModel<KeyValue, string>()
                {
                    IsSuccess = false,
                    Data = null,
                    Error = "Unknown error."
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
        public BaseResponseModel<KeyValue, string> CreateOrUpdateKeyValue([FromBody] KeyValue keyValue)
        {
            BaseResponseModel<KeyValue, string> responseModel;
            bool setResult;
            try
            {
                setResult = RedisCacheService.Set(keyValue.Key, keyValue.Value);
            }
            catch (Exception ex)
            {
                responseModel = new BaseResponseModel<KeyValue, string>()
                {
                    IsSuccess = false,
                    Data = null,
                    Error = ex.ToString()
                };
                return responseModel;
            }

            if (setResult)
            {
                responseModel = new BaseResponseModel<KeyValue, string>()
                {
                    IsSuccess = true,
                    Data = new KeyValue()
                    {
                        Key = keyValue.Key,
                        Value = keyValue.Value
                    },
                    Error = null
                };
            }
            else
            {
                responseModel = new BaseResponseModel<KeyValue, string>()
                {
                    IsSuccess = false,
                    Data = null,
                    Error = "Unknown error."
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
        public BaseResponseModel<string, string> DeleteKeyValueByKey(string key)
        {
            BaseResponseModel<string, string> responseModel;
            bool deleteResult;
            try
            {
                if (!RedisCacheService.ExistKeyValue(key))
                {
                    responseModel = new BaseResponseModel<string, string>()
                    {
                        IsSuccess = false,
                        Data = null,
                        Error = "key doesn't exist."
                    };
                    return responseModel;
                }
                deleteResult = RedisCacheService.Delete(key);
            }
            catch (Exception ex)
            {
                responseModel = new BaseResponseModel<string, string>()
                {
                    IsSuccess = false,
                    Data = null,
                    Error = ex.ToString()
                };
                return responseModel;
            }

            if (deleteResult)
            {
                responseModel = new BaseResponseModel<string, string>()
                {
                    IsSuccess = true,
                    Data = "Delete keyValue success.",
                    Error = null
                };
            }
            else
            {
                responseModel = new BaseResponseModel<string, string>()
                {
                    IsSuccess = false,
                    Data = null,
                    Error = "Unknown error."
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
        public BaseResponseModel<List<string>, string> GetKeysByPattern(string pattern)
        {
            BaseResponseModel<List<string>, string> responseModel;
            try
            {
                List<string> keys = RedisCacheService.GetKeys(pattern);
                responseModel = new BaseResponseModel<List<string>, string>()
                {
                    IsSuccess = true,
                    Data = keys,
                    Error = null
                };
                return responseModel;
            }
            catch (Exception ex)
            {
                responseModel = new BaseResponseModel<List<string>, string>()
                {
                    IsSuccess = false,
                    Data = null,
                    Error = ex.ToString()
                };
                return responseModel;
            }
        }
    }

    /// <summary>
    /// KeyValue
    /// </summary>
    public class KeyValue
    {
        /// <summary>
        /// 鍵
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// 值
        /// </summary>
        public string Value { get; set; }
    }
}