using System;
using System.Text.RegularExpressions;
using Xunit;
using Xunit.Abstractions;

namespace URLRegexTests
{
    public class UnitTest1
    {

        private readonly ITestOutputHelper _output;

        public UnitTest1(ITestOutputHelper output) {
            this._output = output;
        }

        [Theory]
        [InlineData("http://localhost")]
        [InlineData("http://localhost:81")]
        public void Test1(string uri)
        {

            var urlReg = new Regex("^((?<Scheme>.*)://)?((?<Host>.*))?((?<Port>\\d*))?(/(?<Path>.*))?");

            var match = urlReg.Match(uri);

            if (!match.Success)
            {
                throw new Exception($"Can't parse '{uri}'");
            }

            var scheme = match.Groups["Scheme"]?.Value ?? "*";
            var host = match.Groups["Host"]?.Value ?? "*";
            var port = match.Groups["Port"]?.Value ?? "*";
            var path = match.Groups["Path"]?.Value ?? "";

            var nUrl = $"{scheme}://{host}:{port}/{path}";

            _output.WriteLine(nUrl);


        }


        [Theory]
        [InlineData("http://localhost")]
        [InlineData("http://localhost:81")]
        [InlineData("*://localhost:81")]
        public void Test2(string uri)
        {

            

            var nUrl = new Uri(uri);

            _output.WriteLine(nUrl.ToString());


        }
    }
}
