using System.Collections.Generic;

namespace NotBura.Packages
{
    public static class NotGizmosExtensions
    {
        public static void Draw(this NotGizmosProperty property)
        {
            property.Draw();
        }

        public static void Draw(this IEnumerable<NotGizmosProperty> properties)
        {
            if (properties == null)
            {
                return;
            }

            foreach (var property in properties)
            {
                property.Draw();
            }
        }
    }
}
