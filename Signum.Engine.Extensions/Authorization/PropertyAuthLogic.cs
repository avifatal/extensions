﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Signum.Engine.Maps;
using Signum.Entities.Authorization;
using Signum.Entities.Basics;
using Signum.Engine.DynamicQuery;
using Signum.Engine.Basics;
using Signum.Entities;
using Signum.Utilities.DataStructures;
using Signum.Utilities;
using Signum.Entities.DynamicQuery;
using System.Reflection; 

namespace Signum.Engine.Authorization
{
    public static class PropertyAuthLogic
    {
        static AuthCache<RulePropertyDN, PropertyDN, PropertyRoute, PropertyAllowed> cache; 

        public static void Start(SchemaBuilder sb, bool queries)
        {
            if (sb.NotDefined(MethodInfo.GetCurrentMethod()))
            {
                AuthLogic.AssertIsStarted(sb);
                PropertyLogic.Start(sb);

                cache = new AuthCache<RulePropertyDN, PropertyDN, PropertyRoute, PropertyAllowed>(sb,
                    PropertyLogic.GetPropertyRoute,
                    PropertyLogic.GetEntity, MaxPropertyAccess, PropertyAllowed.Modify);

                if (queries)
                {
                    PropertyRoute.SetIsAllowedCallback(pp => GetPropertyAccess(pp) > PropertyAllowed.None);
                }
            }
        }

        static PropertyAllowed MaxPropertyAccess(this IEnumerable<PropertyAllowed> collection)
        {
            return collection.Max();
        }
     
        public static PropertyAllowed GetPropertyAccess(PropertyRoute propertyPath)
        {
            return cache.GetAllowed(RoleDN.Current, propertyPath);
        }

        public static PropertyRulePack GetPropertyRules(Lite<RoleDN> roleLite, TypeDN typeDN)
        {
            var role = roleLite.Retrieve(); 

            return new PropertyRulePack
            {
                Role = roleLite,
                Type = typeDN,
                Rules = cache.GetRules(roleLite, PropertyLogic.RetrieveOrGenerateProperty(typeDN)).ToMList()
            }; 
        }

        public static void SetPropertyRules(PropertyRulePack rules)
        {
            cache.SetRules(rules); 
        }

        public static void SetPermissionAllowed(Lite<RoleDN> role, PropertyRoute property, PropertyAllowed allowed)
        {
            cache.SetAllowed(role, property, allowed);
        }

        public static Dictionary<PropertyRoute, PropertyAllowed> AuthorizedProperties()
        {
            return cache.GetCleanDictionary();
        }
    }
}
