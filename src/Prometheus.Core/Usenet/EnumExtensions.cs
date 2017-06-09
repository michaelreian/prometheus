using System;

namespace Prometheus.Core.Usenet
{
    public static class EnumExtensions
    {
        public static string GetDescription(this ListKeyword listKeyword)
        {
            switch (listKeyword)
            {
                case ListKeyword.Active:
                    return "ACTIVE";
                case ListKeyword.ActiveTimes:
                    return "ACTIVE.TIMES";
                case ListKeyword.DistribPats:
                    return "DISTRIB.PATS";
                case ListKeyword.Headers:
                    return "HEADERS";
                case ListKeyword.NewsGroups:
                    return "NEWSGROUPS";
                case ListKeyword.OverviewFormat:
                    return "OVERVIEW.FMT";
                default:
                    throw new NotImplementedException();
            }
        }
    }
}