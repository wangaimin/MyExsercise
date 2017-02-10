using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace YZ.JsonRpc.AspNet
{
    public class JsonRpcHelpHandler : IHttpHandler
    {
        #region Fields
        /// <summary>
        /// UTF8 Encoding without BOM.
        /// </summary>
        static Encoding UTF8Encoding = new UTF8Encoding(false);
        #endregion

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/html";
            var services = Handler.DefaultHandler.MetaData.Services;
            #region html begin
            context.Response.Write(@"<!DOCTYPE html>
<html lang=""en"">
  <head>
    <meta http-equiv=""Content-Type"" content=""text/html;charset=utf-8"" >
    <TITLE>JSON-RPC help</TITLE>
    <style type=""text/css"">
article,
aside,
details,
figcaption,
figure,
footer,
header,
hgroup,
nav,
section {
  display: block;
}
audio,
canvas,
video {
  display: inline-block;
  *display: inline;
  *zoom: 1;
}
audio:not([controls]) {
  display: none;
}
html {
  font-size: 100%;
  -webkit-text-size-adjust: 100%;
  -ms-text-size-adjust: 100%;
}
a:focus {
  outline: thin dotted #333;
  outline: 5px auto -webkit-focus-ring-color;
  outline-offset: -2px;
}
a:hover,
a:active {
  outline: 0;
}
sub,
sup {
  position: relative;
  font-size: 75%;
  line-height: 0;
  vertical-align: baseline;
}
sup {
  top: -0.5em;
}
sub {
  bottom: -0.25em;
}
img {
  height: auto;
  border: 0;
  -ms-interpolation-mode: bicubic;
  vertical-align: middle;
}
button,
input,
select,
textarea {
  margin: 0;
  font-size: 100%;
  vertical-align: middle;
}
button,
input {
  *overflow: visible;
  line-height: normal;
}
button::-moz-focus-inner,
input::-moz-focus-inner {
  padding: 0;
  border: 0;
}
button,
input[type=""button""],
input[type=""reset""],
input[type=""submit""] {
  cursor: pointer;
  -webkit-appearance: button;
}
input[type=""search""] {
  -webkit-appearance: textfield;
  -webkit-box-sizing: content-box;
  -moz-box-sizing: content-box;
  box-sizing: content-box;
}
input[type=""search""]::-webkit-search-decoration,
input[type=""search""]::-webkit-search-cancel-button {
  -webkit-appearance: none;
}
textarea {
  overflow: auto;
  vertical-align: top;
}
.clearfix {
  *zoom: 1;
}
.clearfix:before,
.clearfix:after {
  display: table;
  content: """";
}
.clearfix:after {
  clear: both;
}
.hide-text {
  overflow: hidden;
  text-indent: 100%;
  white-space: nowrap;
}
.input-block-level {
  display: block;
  width: 100%;
  min-height: 28px;
  /* Make inputs at least the height of their button counterpart */

  /* Makes inputs behave like true block-level elements */

  -webkit-box-sizing: border-box;
  -moz-box-sizing: border-box;
  -ms-box-sizing: border-box;
  box-sizing: border-box;
}
body {
  margin: 0;
  font-family: ""Helvetica Neue"", Helvetica, Arial, sans-serif;
  font-size: 13px;
  line-height: 18px;
  color: #333333;
  background-color: #ffffff;
}
a {
  color: #0088cc;
  text-decoration: none;
}
a:hover {
  color: #005580;
  text-decoration: underline;
}
.row {
  margin-left: -20px;
  *zoom: 1;
}
.row:before,
.row:after {
  display: table;
  content: """";
}
.row:after {
  clear: both;
}
[class*=""span""] {
  float: left;
  margin-left: 20px;
}
.container,
.navbar-fixed-top .container,
.navbar-fixed-bottom .container {
  width: 940px;
}
p {
  margin: 0 0 9px;
  font-family: ""Helvetica Neue"", Helvetica, Arial, sans-serif;
  font-size: 13px;
  line-height: 18px;
}
p small {
  font-size: 11px;
  color: #999999;
}
.lead {
  margin-bottom: 18px;
  font-size: 20px;
  font-weight: 200;
  line-height: 27px;
}
h1,
h2,
h3,
h4,
h5,
h6 {
  margin: 0;
  font-family: inherit;
  font-weight: bold;
  color: inherit;
  text-rendering: optimizelegibility;
}
h1 small,
h2 small,
h3 small,
h4 small,
h5 small,
h6 small {
  font-weight: normal;
  color: #999999;
}
h1 {
  font-size: 30px;
  line-height: 36px;
}
h1 small {
  font-size: 18px;
}
h2 {
  font-size: 24px;
  line-height: 36px;
}
h2 small {
  font-size: 18px;
}
h3 {
  line-height: 27px;
  font-size: 18px;
}
h3 small {
  font-size: 14px;
}
h4,
h5,
h6 {
  line-height: 18px;
}
h4 {
  font-size: 14px;
}
h4 small {
  font-size: 12px;
}
h5 {
  font-size: 12px;
}
h6 {
  font-size: 11px;
  color: #999999;
  text-transform: uppercase;
}
.page-header {
  padding-bottom: 17px;
  margin: 18px 0;
  border-bottom: 1px solid #eeeeee;
}
.page-header h1 {
  line-height: 1;
}
ul,
ol {
  padding: 0;
  margin: 0 0 9px 25px;
}
ul ul,
ul ol,
ol ol,
ol ul {
  margin-bottom: 0;
}
ul {
  list-style: disc;
}
ol {
  list-style: decimal;
}
li {
  line-height: 18px;
}
ul.unstyled,
ol.unstyled {
  margin-left: 0;
  list-style: none;
}
dl {
  margin-bottom: 18px;
}
dt,
dd {
  line-height: 18px;
}
dt {
  font-weight: bold;
  line-height: 17px;
}
dd {
  margin-left: 9px;
}
.dl-horizontal dt {
  float: left;
  clear: left;
  width: 120px;
  text-align: right;
}
.dl-horizontal dd {
  margin-left: 130px;
}
hr {
  margin: 18px 0;
  border: 0;
  border-top: 1px solid #eeeeee;
  border-bottom: 1px solid #ffffff;
}
strong {
  font-weight: bold;
}
em {
  font-style: italic;
}
.muted {
  color: #999999;
}
abbr[title] {
  border-bottom: 1px dotted #ddd;
  cursor: help;
}
abbr.initialism {
  font-size: 90%;
  text-transform: uppercase;
}
blockquote {
  padding: 0 0 0 15px;
  margin: 0 0 18px;
  border-left: 5px solid #eeeeee;
}
blockquote p {
  margin-bottom: 0;
  font-size: 16px;
  font-weight: 300;
  line-height: 22.5px;
}
blockquote small {
  display: block;
  line-height: 18px;
  color: #999999;
}
blockquote small:before {
  content: '\2014 \00A0';
}
blockquote.pull-right {
  float: right;
  padding-left: 0;
  padding-right: 15px;
  border-left: 0;
  border-right: 5px solid #eeeeee;
}
blockquote.pull-right p,
blockquote.pull-right small {
  text-align: right;
}
q:before,
q:after,
blockquote:before,
blockquote:after {
  content: """";
}
address {
  display: block;
  margin-bottom: 18px;
  line-height: 18px;
  font-style: normal;
}
small {
  font-size: 100%;
}
cite {
  font-style: normal;
}
code,
pre {
  padding: 0 3px 2px;
  font-family: Menlo, Monaco, ""Courier New"", monospace;
  font-size: 12px;
  color: #333333;
  -webkit-border-radius: 3px;
  -moz-border-radius: 3px;
  border-radius: 3px;
}
code {
  padding: 2px 4px;
  color: #d14;
  background-color: #f7f7f9;
  border: 1px solid #e1e1e8;
}
pre {
  display: block;
  padding: 8.5px;
  margin: 0 0 9px;
  font-size: 12.025px;
  line-height: 18px;
  background-color: #f5f5f5;
  border: 1px solid #ccc;
  border: 1px solid rgba(0, 0, 0, 0.15);
  -webkit-border-radius: 4px;
  -moz-border-radius: 4px;
  border-radius: 4px;
  white-space: pre;
  white-space: pre-wrap;
  word-break: break-all;
  word-wrap: break-word;
}
pre.prettyprint {
  margin-bottom: 18px;
}
pre code {
  padding: 0;
  color: inherit;
  background-color: transparent;
  border: 0;
}
.pre-scrollable {
  max-height: 340px;
  overflow-y: scroll;
}
.method {
    font-weight: bold;
}
    </style>
  </head>
  <body style=""padding:20px"">");
            #endregion
            context.Response.Write(string.Format("<h3>JSON-RPC Help {0}</h3>", DateTime.Now.ToString("yyyyMMdd")));
            context.Response.Write("<br/>");
            context.Response.Write(System.Environment.NewLine);
            context.Response.Write(System.Environment.NewLine);

            foreach (KeyValuePair<string, SMDService> keyValuePair in services)
            {
                context.Response.Write("<div>");
                context.Response.Write("<p class=\"method\">");

                var service = keyValuePair.Value;
                context.Response.Write(keyValuePair.Key);
                context.Response.Write("</p>");

                context.Response.Write("<pre>");

                JsonRequest jsonRequest = new JsonRequest();
                jsonRequest.Method = keyValuePair.Key;
                jsonRequest.Id = 1;
                jsonRequest.Params = new JObject();
                foreach (var param in keyValuePair.Value.parameters)
                {
                    JToken jObject = null;
                    try
                    {
                        jObject = new JRaw(JsonConvert.SerializeObject(new SampleBuilder().BuildSampleObject(param.ObjectType)));
                    }
                    catch
                    {
                        
                    }
                    ((JObject)jsonRequest.Params).Add(param.Name, jObject);
                }
                context.Response.Write("--> ");
                context.Response.Write(JsonConvert.SerializeObject(jsonRequest));
                context.Response.Write(System.Environment.NewLine);

                JsonResponse jsonResponse = new JsonResponse();
                jsonResponse.Id = 1;
                try
                {
                    jsonResponse.Result = new SampleBuilder().BuildSampleObject(service.returns.ObjectType);
                }
                catch
                {
                    
                }
                context.Response.Write("<-- ");
                try
                {
                    context.Response.Write(JsonConvert.SerializeObject(jsonResponse));
                }
                catch(Exception ex)
                {
                    context.Response.Write(ex.Message);
                }
                context.Response.Write("</pre>");
                context.Response.Write("</div>");

                context.Response.Write(System.Environment.NewLine);
            }

            context.Response.Write("</body></html>");

        }


        public bool IsReusable
        {
            get { return true; }
        }
    }
}
