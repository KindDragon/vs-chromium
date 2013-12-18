﻿// Copyright 2013 The Chromium Authors. All rights reserved.
// Use of this source code is governed by a BSD-style license that can be
// found in the LICENSE file.

using System;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using VsChromiumCore;
using VsChromiumPackage.AutoUpdate;
using VsChromiumPackage.Commands;
using VsChromiumPackage.Features.ChromiumExplorer;
using VsChromiumPackage.Package;
using VsChromiumPackage.Package.CommandHandler;

namespace VsChromiumPackage {
  [PackageRegistration(UseManagedResourcesOnly = true)]
  [InstalledProductRegistration("#110", "#112", "0.9.1", IconResourceID = 400)]
  // When in development mode, update the version # below every time there is a change to the .VSCT file,
  // or Visual Studio won't take into account the changes (this is true with VS 2010, maybe not with
  // VS 2012 and later since package updates is more explicit).
  [ProvideMenuResource("Menus.ctmenu", 7)]
  [ProvideToolWindow(typeof(ChromiumExplorerToolWindow))]
  [Guid(GuidList.GuidVsChromiumPkgString)]
  public sealed class VsPackage : Microsoft.VisualStudio.Shell.Package, IVisualStudioPackage {
    public VsPackage() {
    }

    public IComponentModel ComponentModel { get { return (IComponentModel)GetService(typeof(SComponentModel)); } }

    public OleMenuCommandService OleMenuCommandService { get { return GetService(typeof(IMenuCommandService)) as OleMenuCommandService; } }

    public IVsUIShell VsUIShell { get { return GetService(typeof(SVsUIShell)) as IVsUIShell; } }

    public EnvDTE.DTE DTE { get { return (EnvDTE.DTE)GetService(typeof(EnvDTE.DTE)); } }

    protected override void Initialize() {
      base.Initialize();
      try {
        PreInitialize();
        InitializeCommandHandlers();
        PostInitialize();
      }
      catch (Exception e) {
        Logger.LogException(e, "Error initializing VsChromium package.");
        throw;
      }
    }

    private void PreInitialize() {
      var packageSingletonProvider = ComponentModel.DefaultExportProvider.GetExportedValue<IVisualStudioPackageProvider>();
      packageSingletonProvider.Intialize(this);
    }

    private void InitializeCommandHandlers() {
      var commandHandlerRegistration = ComponentModel.DefaultExportProvider.GetExportedValue<IPackageCommandHandlerRegistration>();
      commandHandlerRegistration.RegisterCommandHandlers();

      var updateChecker = ComponentModel.DefaultExportProvider.GetExportedValue<IUpdateChecker>();
      updateChecker.Start();
    }

    private void PostInitialize() {
    }
  }
}
