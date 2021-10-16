#region Header
//
// Copyright 2003-2021 by Autodesk, Inc.
//
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted,
// provided that the above copyright notice appears in all copies and
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting
// documentation.
//
// AUTODESK PROVIDES THIS PROGRAM "AS IS" AND WITH ALL FAULTS.
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE.  AUTODESK, INC.
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
//
// Use, duplication, or disclosure by the U.S. Government is subject to
// restrictions set forth in FAR 52.227-19 (Commercial Computer
// Software - Restricted Rights) and DFAR 252.227-7013(c)(1)(ii)
// (Rights in Technical Data and Computer Software), as applicable.
//
#endregion // Header

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using RevitLookup.Snoop;
using RevitLookup.Snoop.Forms;

// Each command is implemented as a class that provides the IExternalCommand Interface

namespace RevitLookup
{
  /// <summary>
  /// The classic bare-bones test.  Just brings up an Alert box to show that the connection to the external module is working.
  /// </summary>
  [Transaction( TransactionMode.Manual )]
  public class HelloWorld : IExternalCommand
  {
    public Result Execute( 
      ExternalCommandData cmdData, 
      ref string msg, 
      ElementSet elems )
    {
      Assembly a = Assembly.GetExecutingAssembly();
      string version = a.GetName().Version.ToString();
      TaskDialog helloDlg = new TaskDialog( "Autodesk Revit" );
      helloDlg.MainContent = "Hello World from " + a.Location + " v" + version;
      helloDlg.Show();
      return Result.Cancelled;
    }
  }

  /// <summary>
  /// SnoopDB command:  Browse all Elements in the current Document
  /// </summary>

  [Transaction( TransactionMode.Manual )]
  public class CmdSnoopDb : IExternalCommand
  {
    public Result Execute( 
      ExternalCommandData cmdData, 
      ref string msg, 
      ElementSet elems )
    {
          var objs = Selectors.SnoopDB(cmdData.Application);
          Snoop.Forms.Objects form = new Snoop.Forms.Objects( objs );         
          ModelessWindowFactory.Show(form);  

          return Result.Succeeded;
    }
  }

  [Transaction( TransactionMode.Manual )]
  public class CmdSnoopModScopePickSurface : IExternalCommand
  {
    public Result Execute( 
      ExternalCommandData cmdData, 
      ref string msg, 
      ElementSet elems )
    {
        object refElem = Selectors.SnoopPickFace(cmdData.Application);
        if (refElem == null) return Result.Cancelled;

        Snoop.Forms.Objects form = new Snoop.Forms.Objects( refElem );       
        ModelessWindowFactory.Show(form);       

        return Result.Succeeded;
    }
  }

  [Transaction( TransactionMode.Manual )]
  public class CmdSnoopModScopePickEdge : IExternalCommand
  {
    public Result Execute( ExternalCommandData cmdData, ref string msg, ElementSet elems )
    {
        object refElem = Selectors.SnoopPickEdge(cmdData.Application) as object;
        if (refElem == null) return Result.Cancelled;

        Snoop.Forms.Objects form = new Snoop.Forms.Objects( refElem );       
        ModelessWindowFactory.Show(form);     

        return Result.Succeeded;
    }
  }

  [Transaction( TransactionMode.Manual )]
  public class CmdSnoopModScopeLinkedElement : IExternalCommand
  {
    public Result Execute( ExternalCommandData cmdData, ref string msg, ElementSet elems )
    {
        var e = Selectors.SnoopLinkedElement(cmdData.Application) as Element;
        if (e == null) return Result.Cancelled;

        Snoop.Forms.Objects form = new Snoop.Forms.Objects( e );      
        ModelessWindowFactory.Show(form);
        
        return Result.Succeeded;
    }
  }

  /// <summary>
  /// Snoop dependent elements using
  /// Element.GetDependentElements
  /// </summary>
  [Transaction( TransactionMode.Manual )]
  public class CmdSnoopModScopeDependents : IExternalCommand
  {
    public Result Execute(
      ExternalCommandData cmdData,
      ref string msg,
      ElementSet elems )
    {
        var elements = Selectors.SnoopDependentElements(cmdData.Application);
        Snoop.Forms.Objects form = new Snoop.Forms.Objects(elements);        
        ModelessWindowFactory.Show(form);

        return Result.Succeeded;
    }
  }

  /// <summary>
  /// SnoopDB command:  Browse the current view...
  /// </summary>
  [Transaction( TransactionMode.Manual )]
  public class CmdSnoopActiveView : IExternalCommand
  {
    public Result Execute( ExternalCommandData cmdData, ref string msg, ElementSet elems )
    {
        var activeView = Selectors.SnoopActiveView(cmdData.Application);
        if ( activeView == null ) return Result.Cancelled;

        Snoop.Forms.Objects form = new Snoop.Forms.Objects(activeView);
        ModelessWindowFactory.Show(form);
      
        return Result.Succeeded;
    }
  }

  /// <summary>
  /// Snoop ModScope command:  Browse all Elements in the current selection set
  ///                          In case nothing is selected: browse visible elements
  /// </summary>
  [Transaction( TransactionMode.Manual )]
  public class CmdSnoopModScope : IExternalCommand
  {
        public Result Execute(ExternalCommandData cmdData, ref string msg, ElementSet elems)
        {
            var selected = Selectors.SnoopCurrentSelection(cmdData.Application);

            Snoop.Forms.Objects form = new Snoop.Forms.Objects(selected);
            ModelessWindowFactory.Show(form);

            return Result.Succeeded;
        }
    }

  /// <summary>
  /// Snoop App command:  Browse all objects that are part of the Application object
  /// </summary>
  [Transaction( TransactionMode.Manual )]
  public class CmdSnoopApp : IExternalCommand
  {
    public Result Execute( ExternalCommandData cmdData, ref string msg, ElementSet elems )
    {
         var app = Selectors.SnoopApplication(cmdData.Application);

         Snoop.Forms.Objects form = new Snoop.Forms.Objects(app);
         ModelessWindowFactory.Show(form);

         return Result.Succeeded;
    }
  }

  /// <summary>
  /// Snoop ModScope command:  Browse all Elements in the current selection set
  /// </summary>
  [Transaction( TransactionMode.Manual )]
  public class CmdSampleMenuItemCallback : IExternalCommand
  {
    public Result Execute( ExternalCommandData cmdData, ref string msg, ElementSet elems )
    {
      Result result;

      try
      {
        MessageBox.Show( "Called back into RevitLookup by picking toolbar or menu item" );
        result = Result.Succeeded;
      }
      catch( System.Exception e )
      {
        msg = e.Message;
        result = Result.Failed;
      }

      return result;
    }
  }

  /// <summary>
  /// Search by and Snoop command: Browse
  /// elements found by the condition
  /// </summary>
  [Transaction( TransactionMode.Manual )]
  public class CmdSearchBy : IExternalCommand
  {
    public Result Execute(
      ExternalCommandData cmdData,
      ref string msg,
      ElementSet elems )
    {        
        UIDocument revitDoc = cmdData.Application.ActiveUIDocument;
        Document dbdoc = revitDoc.Document;
        SearchBy form = new SearchBy( dbdoc );
        ModelessWindowFactory.Show(form);

        return Result.Succeeded;
    }
  }
}
