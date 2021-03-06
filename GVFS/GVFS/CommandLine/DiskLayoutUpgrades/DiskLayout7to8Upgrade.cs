﻿using GVFS.Common;
using GVFS.Common.Tracing;
using Microsoft.Isam.Esent;
using Microsoft.Isam.Esent.Collections.Generic;
using System.IO;

namespace GVFS.CommandLine.DiskLayoutUpgrades
{
    public class DiskLayout7to8Upgrade : DiskLayoutUpgrade
    {
        protected override int SourceLayoutVersion
        {
            get { return 7; }
        }

        /// <summary>
        /// Version 7 to 8 only added a new value to BackgroundGitUpdate.OperationType,
        /// so we only need to bump the ESENT version here.
        /// </summary>
        public override bool TryUpgrade(ITracer tracer, string enlistmentRoot)
        {
            string dotGVFSRoot = Path.Combine(enlistmentRoot, GVFSConstants.DotGVFS.Root);
            string esentRepoMetadata = Path.Combine(dotGVFSRoot, EsentRepoMetadataName);
            try
            {
                using (PersistentDictionary<string, string> esentMetadata = new PersistentDictionary<string, string>(esentRepoMetadata))
                {
                    esentMetadata[DiskLayoutUpgrade.DiskLayoutEsentVersionKey] = "8";
                }
            }
            catch (EsentException ex)
            {
                tracer.RelatedError("RepoMetadata appears to be from an older version of GVFS and corrupted: " + ex.Message);
                return false;
            }

            // Do not call TryIncrementDiskLayoutVersion. It updates the flat repo metadata which does not exist yet.
            return true;
        }
    }
}