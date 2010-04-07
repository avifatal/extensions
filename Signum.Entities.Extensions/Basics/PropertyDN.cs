﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Signum.Utilities;
using System.Reflection;

namespace Signum.Entities.Basics
{
    public class PropertyDN : IdentifiableEntity
    {
        public PropertyDN() { }

        [field: Ignore]
        PropertyRoute route;
        public PropertyRoute Route
        {
            get { return route; }
            set { route = value; }
        }

        [NotNullable, SqlDbType(Size = 100)]
        string path;
        [StringLengthValidator(AllowNulls = false, Min = 1, Max = 100)]
        public string Path
        {
            get { return path; }
            set { SetToStr(ref path, value, () => Path); }
        }

        TypeDN type;
        [NotNullValidator]
        public TypeDN Type
        {
            get { return type; }
            set { Set(ref type, value, () => Type); }
        }

        public override string ToString()
        {
            return path;
        }
    }
}
