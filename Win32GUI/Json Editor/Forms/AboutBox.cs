using System.Reflection;

namespace ZTn.Json.Editor.Forms
{
    partial class AboutBox : Form
    {
        public AboutBox()
        {
            InitializeComponent();
            Text = $"À propos de {AssemblyTitle}";
            labelProductName.Text = AssemblyProduct;
            labelVersion.Text = $"Version {AssemblyVersion}";
            labelCopyright.Text = AssemblyCopyright;
            labelCompanyName.Text = AssemblyCompany;
            textBoxDescription.Text = AssemblyDescription;
        }

        #region Accesseurs d'attribut de l'assembly

        public static string AssemblyTitle
        {
            get
            {
                var attributes = Assembly
                    .GetExecutingAssembly()
                    .GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                if (attributes.Length > 0)
                {
                    var titleAttribute = (AssemblyTitleAttribute)attributes[0];
                    if (titleAttribute.Title != "")
                    {
                        return titleAttribute.Title;
                    }
                }
                return Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
            }
        }

        public static string AssemblyVersion =>
            Assembly.GetExecutingAssembly().GetName().Version.ToString();

        public static string AssemblyDescription
        {
            get
            {
                var attributes = Assembly
                    .GetExecutingAssembly()
                    .GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
                return attributes.Length == 0
                    ? ""
                    : ((AssemblyDescriptionAttribute)attributes[0]).Description;
            }
        }

        public static string AssemblyProduct
        {
            get
            {
                var attributes = Assembly
                    .GetExecutingAssembly()
                    .GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                return attributes.Length == 0
                    ? ""
                    : ((AssemblyProductAttribute)attributes[0]).Product;
            }
        }

        public static string AssemblyCopyright
        {
            get
            {
                var attributes = Assembly
                    .GetExecutingAssembly()
                    .GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
                return attributes.Length == 0
                    ? ""
                    : ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
            }
        }

        public static string AssemblyCompany
        {
            get
            {
                var attributes = Assembly
                    .GetExecutingAssembly()
                    .GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
                return attributes.Length == 0
                    ? ""
                    : ((AssemblyCompanyAttribute)attributes[0]).Company;
            }
        }
        #endregion
    }
}
