// Public snake_case helper
using System.Text;

namespace SnakeTheBite.Scripts;

public static class SnakeTheBiteModelHelper
{
    //
    // 将 PascalCase 文本转换为 snake_case。
    // 例如：TestCard -> test_card，SnakeBiteThornsPower -> snake_bite_thorns_power
    //
    public static string ToSnakeCase(string text)
    {
        if (string.IsNullOrEmpty(text))
            return text;

        StringBuilder result = new StringBuilder();
        result.Append(char.ToLowerInvariant(text[0]));

        for (int i = 1; i < text.Length; i++)
        {
            char c = text[i];
            if (char.IsUpper(c))
            {
                result.Append('_');
                result.Append(char.ToLowerInvariant(c));
            }
            else
            {
                result.Append(c);
            }
        }

        return result.ToString();
    }
}
