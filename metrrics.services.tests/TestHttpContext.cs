using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Http.Features;

namespace metrics.services.tests
{
    public class TestHttpContext : HttpContext
    {
        public override void Abort()
        {
            throw new NotImplementedException();
        }

        public override IFeatureCollection Features { get; }
        public override HttpRequest Request { get; }
        public override HttpResponse Response { get; }
        public override ConnectionInfo Connection { get; }
        public override WebSocketManager WebSockets { get; }
        public override AuthenticationManager Authentication { get; }

        public override ClaimsPrincipal User
        {
            get
            {
                var claims = new List<Claim>
                {
                    new Claim(Constants.VK_TOKEN_CLAIM, "7394c4d95e53d1b0853166482976d80d3a31efb3462de02ccd492cd19d8d9b32ed1633c12b5d0c88e84c8")
                };
                var claimsIdentity = new ClaimsIdentity(claims);
                return new ClaimsPrincipal(claimsIdentity);
            }
            set
            {
                
            }
        }

        public override IDictionary<object, object> Items { get; set; }
        public override IServiceProvider RequestServices { get; set; }
        public override CancellationToken RequestAborted { get; set; }
        public override string TraceIdentifier { get; set; }
        public override ISession Session { get; set; }
    }
}