using System;
using FluentNHibernate.Diagnostics;

namespace NHibernate.vNext
{
    public interface IDatabaseFactory 
    {
        ISession Session { get; }
        IDatabaseRequest BeginRequest(bool beginTransaction = true);
    }
}
