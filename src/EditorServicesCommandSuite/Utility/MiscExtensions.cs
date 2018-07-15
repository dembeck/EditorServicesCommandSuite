using System.Globalization;
using System.Management.Automation;
using System.Management.Automation.Internal;

namespace EditorServicesCommandSuite.Utility
{
    internal static class MiscExtensions
    {
        internal static void ThrowIfStopping(this PSCmdlet cmdlet)
        {
            if (cmdlet == null || !cmdlet.Stopping)
            {
                return;
            }

            throw new PipelineStoppedException();
        }

        internal static PSObject Invoke(this PSObject pso, string methodName, params object[] args)
        {
            return PSObject.AsPSObject(
                pso.Methods[methodName]?.Invoke(args)
                ?? AutomationNull.Value);
        }

        internal static string FormatInvariant(this string format, object arg0)
        {
            return string.Format(
                CultureInfo.InvariantCulture,
                format,
                arg0);
        }

        internal static string FormatInvariant(this string format, object arg0, object arg1)
        {
            return string.Format(
                CultureInfo.InvariantCulture,
                format,
                arg0,
                arg1);
        }

        internal static string FormatInvariant(this string format, object arg0, object arg1, object arg2)
        {
            return string.Format(
                CultureInfo.InvariantCulture,
                format,
                arg0,
                arg1,
                arg2);
        }

        internal static string FormatInvariant(this string format, params object[] args)
        {
            return string.Format(
                CultureInfo.InvariantCulture,
                format,
                args);
        }

        internal static string FormatCulture(this string format, object arg0)
        {
            return string.Format(
                CultureInfo.CurrentCulture,
                format,
                arg0);
        }

        internal static string FormatCulture(this string format, object arg0, object arg1)
        {
            return string.Format(
                CultureInfo.CurrentCulture,
                format,
                arg0,
                arg1);
        }

        internal static string FormatCulture(this string format, object arg0, object arg1, object arg2)
        {
            return string.Format(
                CultureInfo.CurrentCulture,
                format,
                arg0,
                arg1,
                arg2);
        }

        internal static string FormatCulture(this string format, params object[] args)
        {
            return string.Format(
                CultureInfo.CurrentCulture,
                format,
                args);
        }
    }
}
