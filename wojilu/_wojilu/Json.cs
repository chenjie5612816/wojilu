﻿/*
 * Copyright 2010 www.wojilu.com
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *      http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Collections.Generic;
using System.Text;
using wojilu.Serialization;
using System.Collections;

namespace wojilu {

    public class Json {

        /// <summary>
        /// 将对象转换成 json 字符串
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static String Serialize( Object obj ) {
            return JsonString.Convert( obj );
        }

        /// <summary>
        /// 将对象转换成 json 字符串
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="isBreakline">是否换行(默认不换行，阅读起来更加清晰)</param>
        /// <returns></returns>
        public static String Serialize( Object obj, Boolean isBreakline ) {
            return JsonString.Convert( obj, isBreakline );
        }
        
        /// <summary>
        /// 将对象数组转换成 json 字符串
        /// </summary>
        /// <param name="arrObj"></param>
        /// <returns></returns>
        public static String SerializeArray( object[] arrObj ) {
            return JsonString.Convert( arrObj );
        }

        /// <summary>
        /// 将对象列表转换成 json 字符串
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static String SerializeList( IList list ) {
            return JsonString.Convert( list );
        }

        /// <summary>
        /// 将对象列表转换成 json 字符串
        /// </summary>
        /// <param name="list"></param>
        /// <param name="isBreakline">是否换行(默认不换行，阅读起来更加清晰)</param>
        /// <returns></returns>
        public static String SerializeList( IList list, Boolean isBreakline ) {
            return JsonString.Convert( list, isBreakline );
        }

        /// <summary>
        /// 将字典 Dictionary 转换成 json 字符串
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        public static String SerializeDic( IDictionary dic ) {
            return JsonString.Convert( dic );
        }

        /// <summary>
        /// 将字典 Dictionary 转换成 json 字符串
        /// </summary>
        /// <param name="dic"></param>
        /// <param name="isBreakline">是否换行(默认不换行，阅读起来更加清晰)</param>
        /// <returns></returns>
        public static String SerializeDic( IDictionary dic, Boolean isBreakline ) {
            return JsonString.Convert( dic, isBreakline );
        }
        
        /// <summary>
        /// 将对象转换成 json 字符串
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static String SerializeObject( Object obj ) {
            return JsonString.Convert( obj );
        }

        /// <summary>
        /// 将对象转换成 json 字符串
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="isBreakline">是否换行(默认不换行，阅读起来更加清晰)</param>
        /// <returns></returns>
        public static String SerializeObject( Object obj, Boolean isBreakline ) {
            return JsonString.Convert( obj, isBreakline );
        }

        /// <summary>
        /// 将 ORM 的实体类转换成 json 字符串
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static String SerializeEntity( IEntity obj ) {
            return JsonString.Convert( obj );
        }

        /// <summary>
        /// 将简单对象列表转换成 json 字符串(不换行，不支持子对象)
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static String SerializeListSimple( IList list ) {
            return SimpleJsonString.ConvertList( list );
        }

        /// <summary>
        /// 将简单对象转换成 json 字符串(不换行，不支持子对象)
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static String SerializeObjectSimple( Object obj ) {
            return SimpleJsonString.ConvertObject( obj );
        }

        /// <summary>
        /// 解析字符串，返回原始的 json 类型对象。
        /// 根据 json 的不同，可能返回整数(int)、布尔类型(bool)、字符串(string)、一般Json对象(JsonObject)、数组(List&lt;object&gt;)等不同类型
        /// </summary>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        public static Object Deserialize( String jsonString ) {
            if (strUtil.IsNullOrEmpty( jsonString )) return null;
            return JsonParser.Parse( jsonString );
        }

        /// <summary>
        /// 将 json 字符串反序列化为原始的 JsonObject 类型
        /// </summary>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        public static JsonObject DeserializeJson( String jsonString ) {
            if (strUtil.IsNullOrEmpty( jsonString )) return new JsonObject();
            return JSON.ToDictionary( jsonString );
        }

        /// <summary>
        /// 将 json 字符串解析为 json 原始数据类型的列表，比如 ["abc", 88, {name:"aa", gender:"male"}]
        /// </summary>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        public static List<Object> DeserializeJsonList( String jsonString ) {
            if (strUtil.IsNullOrEmpty( jsonString )) return new List<Object>();
            return JsonParser.Parse( jsonString ) as List<Object>;
        }

        /// <summary>
        /// 将 json 字符串反序列化为 原始的json强类型(int,string,JsonObject等) 的数据列表。
        /// 当列表内数据类型相同时使用，比如 ["abc", "www", "qqqxyz", "uuu"]
        /// </summary>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        public static List<T> DeserializeJsonList<T>( String jsonString ) {
            if (strUtil.IsNullOrEmpty( jsonString )) return new List<T>();
            List<Object> list = DeserializeJsonList( jsonString );            
            return JSON.ConvertList<T>( list);
        }

        /// <summary>
        /// 将 json 字符串反序列化为对象
        /// </summary>
        /// <param name="jsonString">json 字符串</param>
        /// <param name="t">目标类型</param>
        /// <returns></returns>
        public static Object DeserializeObject( String jsonString, Type t ) {
            return JSON.ToObject( jsonString, t );
        }

        /// <summary>
        /// 将 json 字符串反序列化为对象
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <param name="jsonString">json 字符串</param>
        /// <returns></returns>
        public static T DeserializeObject<T>( String jsonString ) {
            return JSON.ToObject<T>( jsonString );
        }

        /// <summary>
        /// 将 json 字符串反序列化为对象列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        public static List<T> DeserializeList<T>( String jsonString ) {
            return JSON.ToList<T>( jsonString );
        }

        /// <summary>
        /// 将 json 字符串反序列化为 ORM 实体类
        /// </summary>
        /// <param name="jsonString"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static IEntity DeserializeObjectEntity( String jsonString, Type t ) {
            return JSON.ToEntity( jsonString, t );
        }



    }

}
