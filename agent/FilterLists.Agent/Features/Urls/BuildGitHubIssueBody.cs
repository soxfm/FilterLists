﻿using System.Collections.Generic;
using System.Linq;
using System.Text;
using FilterLists.Agent.Core.Urls;
using MediatR;

namespace FilterLists.Agent.Features.Urls
{
    public static class BuildGitHubIssueBody
    {
        public class Command : IRequest<string>
        {
            public Command(IEnumerable<EntityUrl> invalidEntityUrls)
            {
                InvalidEntityUrls = invalidEntityUrls;
            }

            public IEnumerable<EntityUrl> InvalidEntityUrls { get; }
        }

        public class Handler : RequestHandler<Command, string>
        {
            private const string IssueHeader =
                "<p>This issue is auto-generated by the <a href=\"https://github.com/collinbarrett/FilterLists/tree/master/src/FilterLists.Agent\">FilterLists.Agent</a>. It is updated at about 9am UTC daily.</p><p>We rely on the help of the community to ensure that the FilterLists site data remains up-to-date. The URLs listed below have been automatically flagged and may <a href=\"https://github.com/collinbarrett/FilterLists/tree/master/data\">need to be updated</a>. Please consider submitting a PR against this issue updating some or all of the URLs accordingly.</p><p>Thanks for your contributions!</p>";

            protected override string Handle(Command request)
            {
                var body = new StringBuilder();
                body.Append(IssueHeader);
                var entityInvalidUrlGroups = request.InvalidEntityUrls.GroupBy(i => i.Entity);
                foreach (var entityInvalidUrls in entityInvalidUrlGroups)
                {
                    body.Append(
                        $"<h1><a href=\"https://github.com/collinbarrett/FilterLists/blob/master/data/{entityInvalidUrls.Key}.json\">{entityInvalidUrls.Key}.json</a></h1>");
                    body.Append("<ul>");
                    foreach (var invalidUrl in entityInvalidUrls)
                    {
                        body.Append(
                            $"<li><a href=\"{invalidUrl.Url.OriginalString}\">{invalidUrl.Url.OriginalString}</a>");
                        body.Append("<ul>");
                        foreach (var message in invalidUrl.ValidationMessages)
                            body.Append($"<li>{message}</li>");
                        body.Append("</ul>");
                        body.Append("</li>");
                    }

                    body.Append("</ul>");
                }

                return body.ToString();
            }
        }
    }
}