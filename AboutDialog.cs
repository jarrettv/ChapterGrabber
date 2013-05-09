using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace JarrettVance.ChapterTools
{
  partial class AboutDialog : Form
  {
    public AboutDialog()
    {
      InitializeComponent();
      this.Text = String.Format("About {0}", AssemblyTitle);
      this.labelProductName.Text = AssemblyProduct;
      this.labelVersion.Text = String.Format("Version {0}", AssemblyVersion);
      this.labelCopyright.Text = AssemblyCopyright;
      this.linkWeb.Text = AssemblyCompany;
      this.textBoxDescription.Text = AssemblyDescription;
      this.textBoxDescription.Text += Environment.NewLine + Environment.NewLine +
        "ChapterGrabber by Jarrett Vance" + Environment.NewLine +
        "IFO parsing inspired by OpenMediaLibrary" + Environment.NewLine +
        "M2TS parsing inspired by BDInfo" + Environment.NewLine +
        "Icons by famfamfam" + Environment.NewLine +
        "Chapter titles by tagchimp" + Environment.NewLine +
        "Chapter titles by Kristian";
    }

    #region Assembly Attribute Accessors

    public string AssemblyTitle
    {
      get
      {
        object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
        if (attributes.Length > 0)
        {
          AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
          if (titleAttribute.Title != "")
          {
            return titleAttribute.Title;
          }
        }
        return System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
      }
    }

    public string AssemblyVersion
    {
      get
      {
        return Assembly.GetExecutingAssembly().GetName().Version.ToString();
      }
    }

    public string AssemblyDescription
    {
      get
      {
        object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
        if (attributes.Length == 0)
        {
          return "";
        }
        return ((AssemblyDescriptionAttribute)attributes[0]).Description;
      }
    }

    public string AssemblyProduct
    {
      get
      {
        object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
        if (attributes.Length == 0)
        {
          return "";
        }
        return ((AssemblyProductAttribute)attributes[0]).Product;
      }
    }

    public string AssemblyCopyright
    {
      get
      {
        object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
        if (attributes.Length == 0)
        {
          return "";
        }
        return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
      }
    }

    public string AssemblyCompany
    {
      get
      {
        object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
        if (attributes.Length == 0)
        {
          return "";
        }
        return ((AssemblyCompanyAttribute)attributes[0]).Company;
      }
    }
    #endregion

    private void linkWeb_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
      // Change the color of the link text by setting LinkVisited 
      // to true.
      linkWeb.LinkVisited = true;
      //Call the Process.Start method to open the default browser 
      //with a URL:
      System.Diagnostics.Process.Start(linkWeb.Text);
    }
  }
}
