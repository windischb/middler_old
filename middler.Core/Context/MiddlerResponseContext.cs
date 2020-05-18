﻿using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using middler.Common;
using middler.Common.ExtensionMethods;
using middler.Common.Interfaces;
using middler.Common.SharedModels.Models;
using middler.Common.StreamHelper;
using middler.Common.ExtensionMethods;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace middler.Core.Context
{
    public class MiddlerResponseContext : IMiddlerResponseContext
    {
        //private FakeHttpContext FakeHttpContext { get; set; }

        public int StatusCode { get; set; }
        public SimpleDictionary<string> Headers { get; set; } = new SimpleDictionary<string>();

        public Stream Body { get; internal set; }


        public MiddlerResponseContext()
        {

        }

        public string GetBodyAsString()
        {

            using var sr = new StreamReader(Body);
            Body.Seek(0, SeekOrigin.Begin);
            return sr.ReadToEnd();

        }

        public void SetBody(object body)
        {

            Body.Seek(0, SeekOrigin.Begin);
            
            Body.SetLength(0);

            switch (body)
            {
                case string str:
                    {
                        SetStringBody(str);
                        return;
                    }
                case Stream stream:
                    {
                        SetStreamBody(stream);
                        return;
                    }
                default:
                    {
                        SetObjectBody(body);
                        break;
                    }
            }


        }

        private void SetObjectBody(object @object)
        {
            using var sw = new StreamWriter(Body, Encoding.UTF8, 8192, true);
            JsonSerializer serializer = new JsonSerializer();
            serializer.Serialize(sw, @object);

        }

        private void SetStringBody(string content)
        {
            using var sw = new StreamWriter(Body, Encoding.UTF8, 8192, true);
            sw.Write(content);
        }

        private void SetStreamBody(Stream stream)
        {
            Body = stream;
        }
    }
}
