﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace DotNetUtils
{
    public class EmbededXmlResolver : XmlUrlResolver
    {
        private readonly Assembly _resourceAssembly = null;
        private readonly Uri _baseUri;

        public EmbededXmlResolver(Assembly resourceAssembly, string baseUri)
        {
            if (resourceAssembly == null)
            {
                throw new ArgumentNullException("resourceAssembly");
            }
            _resourceAssembly = resourceAssembly;
            _baseUri = new Uri("res://local/" + baseUri);
        }

        public override Uri ResolveUri(Uri baseUri, string relativeUri)
        {
            return base.ResolveUri(_baseUri, relativeUri);
        }

        override public object GetEntity(Uri absoluteUri, string role, Type type)
        {
            if (absoluteUri.Scheme == "res")
            {
                var path = absoluteUri.LocalPath.Replace("/", ".").Replace(" ", "_").TrimStart('.');
                return _resourceAssembly.GetManifestResourceStream(path);
            }
            return null;
        }
    }
}