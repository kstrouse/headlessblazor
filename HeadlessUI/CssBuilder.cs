using System;

namespace HeadlessUI
{
    public class CssBuilder<T>
    {
        public static string Build(T context, string defaultValue = "", Action<CssBuilder<T>> action = null)
        {
            var builder = new CssBuilder<T>(context, defaultValue);
            action?.Invoke(builder);
            return builder.Build();
        }

        public static CssBuilder<T> Default(T context, string value) => new CssBuilder<T>(context, value);
        public static CssBuilder<T> Empty(T context) => new CssBuilder<T>(context);

        private string stringBuffer;
        private readonly T context;

        public CssBuilder(T context, string defaultValue = "")
        {
            this.context = context;
            stringBuffer = defaultValue;
        }

                
        public CssBuilder<T> Add(string value, bool when = true) => when ? AddValue(" " + value) : this;
        public CssBuilder<T> Add(string value, Func<T, bool> when) => Add(value, when(context));
        public CssBuilder<T> Add(Func<string> value, bool when = true) => when ? Add(value()) : this;
        public CssBuilder<T> Add(Func<string> value, Func<T, bool> when) => Add(value, when(context));

        public string Build()
        {
            // String buffer finalization code
            return stringBuffer != null ? stringBuffer.Trim() : string.Empty;
        }

        private CssBuilder<T> AddValue(string value)
        {
            stringBuffer += value;
            return this;
        }

        // ToString should only and always call Build to finalize the rendered string.
        public override string ToString() => Build();

        public static implicit operator CssBuilder<T>(string classes) => new CssBuilder<T>(default, classes);

    }
}
