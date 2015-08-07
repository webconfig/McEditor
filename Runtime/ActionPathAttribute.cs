
using System;
public class ActionPathAttribute:Attribute
{
    public readonly string path;

		public string Path
		{
			get
			{
				return this.path;
			}
		}

        public ActionPathAttribute(string _path)
		{
            path = _path;
		}
}

