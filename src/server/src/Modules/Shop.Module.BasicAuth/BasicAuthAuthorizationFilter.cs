using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using Shop.Module.BasicAuth.Dashboard;

namespace Shop.Module.BasicAuth
{
    public class BasicAuthAuthorizationFilter : IDashboardAuthorizationFilter
    {
        private readonly BasicAuthAuthorizationFilterOptions _options;

        public BasicAuthAuthorizationFilter()
            : this(new BasicAuthAuthorizationFilterOptions())
        {
        }

        public BasicAuthAuthorizationFilter(BasicAuthAuthorizationFilterOptions options)
        {
            _options = options;
        }

        private bool Challenge(HttpContext context)
        {
            context.Response.StatusCode = 401;
            context.Response.Headers.Append("WWW-Authenticate", "Basic realm=\"BaseAuth Dashboard\"");
            return false;
        }

        public bool Authorize(DashboardContext _context)
        {

            var context = _context.GetHttpContext();
            if ((_options.SslRedirect == true) && (context.Request.Scheme != "https"))
            {
                string redirectUri = new UriBuilder("https", context.Request.Host.ToString(), 443, context.Request.Path).ToString();

                context.Response.StatusCode = 301;
                context.Response.Redirect(redirectUri);
                return false;
            }

            if ((_options.RequireSsl == true) && (context.Request.IsHttps == false))
            {
                return false;
            }

            string header = context.Request.Headers["Authorization"];

            if (string.IsNullOrWhiteSpace(header) == false)
            {
                AuthenticationHeaderValue authValues = AuthenticationHeaderValue.Parse(header);

                if ("Basic".Equals(authValues.Scheme, StringComparison.OrdinalIgnoreCase))
                {
                    string parameter = Encoding.UTF8.GetString(Convert.FromBase64String(authValues.Parameter));
                    var parts = parameter.Split(':');

                    if (parts.Length > 1)
                    {
                        string login = parts[0];
                        string password = parts[1];

                        if ((string.IsNullOrWhiteSpace(login) == false) && (String.IsNullOrWhiteSpace(password) == false))
                        {
                            return _options
                                .Users
                                .Any(user => user.Validate(login, password, _options.LoginCaseSensitive))
                                   || Challenge(context);
                        }
                    }
                }
            }

            return Challenge(context);
        }
    }
}