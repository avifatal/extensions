﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Signum.Entities.Omnibox;
using Signum.Windows.Authorization;
using Signum.Entities.DynamicQuery;
using Signum.Entities;
using Signum.Services;
using System.Reflection;
using System.Windows.Documents;
using System.Windows;
using System.Linq.Expressions;
using Signum.Utilities;

namespace Signum.Windows.Omnibox
{
    public static class OmniboxClient
    {
        public static Dictionary<Type, OmniboxProviderBase> Providers = new Dictionary<Type,OmniboxProviderBase>();
        public static void Start()
        {
            if (Navigator.Manager.NotDefined(MethodInfo.GetCurrentMethod()))
            {
                OmniboxParser.Manager = new WindowsOmniboxManager();
            }
        }

        public static void Register<T>(this OmniboxProvider<T> provider) where T : OmniboxResult
        {
            OmniboxParser.Generators.Add(provider.CreateGenerator());
            Providers[typeof(T)] = provider;
        }

        public static void AddMatch(this InlineCollection lines, OmniboxMatch match)
        {
            foreach (var item in match.BoldSpans())
            {
                var run = new Run(item.Item1);

                lines.Add(item.Item2 ? (Inline)new Bold(run) : run);
            }
        }
    }

    public abstract class OmniboxProviderBase
    {
        public abstract void RenderLinesBase(OmniboxResult result, InlineCollection lines);

        public abstract void OnSelectedBase(OmniboxResult result, Window window);

        public abstract string GetItemStatusBase(OmniboxResult result);

        public abstract Run GetIcon();

    }

    public abstract class OmniboxProvider<T> : OmniboxProviderBase where T : OmniboxResult
    {
        public abstract OmniboxResultGenerator<T> CreateGenerator();

        public abstract void RenderLines(T result, InlineCollection lines);

        public abstract void OnSelected(T result, Window window);

        public abstract string GetItemStatus(T result);
    
        public override void RenderLinesBase(OmniboxResult result, InlineCollection lines)
        {
            RenderLines((T)result, lines); 
        }

        public override void OnSelectedBase(OmniboxResult result, Window window)
        {   
            OnSelected((T)result, window);
        }

        public override string GetItemStatusBase(OmniboxResult result)
        {
            return GetItemStatus((T)result);
        }
    }

    public class WindowsOmniboxManager : OmniboxManager
    {
        public override bool AllowedType(Type type)
        {
            return Navigator.IsNavigable(type, isSearchEntity: false);
        }

        public override Lite RetrieveLite(Type type, int id)
        {
            if (!Server.Return((IBaseServer bs) => bs.Exists(type, id)))
                return null;
            return Server.FillToStr(Lite.Create(type, id));
        }

        public override bool AllowedQuery(object queryName)
        {
            return Navigator.IsFindable(queryName);
        }

        public override QueryDescription GetDescription(object queryName)
        {
            return Navigator.Manager.GetQueryDescription(queryName);
        }

        public override List<Lite> AutoComplete(Type cleanType, Implementations implementations, string subString, int count)
        {
            if (string.IsNullOrEmpty(subString))
                return new List<Lite>();

            return Server.Return((IBaseServer bs) => bs.FindLiteLike(cleanType, implementations, subString, 5));
        }
    }
}
