using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace BuildReportRemastered.Window.Screen
{
    public partial class AssetList
    {
        bool ShouldShowDeleteButtons(BuildInfo buildReportToDisplay)
        {
            return
                (IsShowingUnusedAssets && buildReportToDisplay.HasUnusedAssets) ||
                (buildReportToDisplay.HasUsedAssets && BuildReportRemastered.Options.AllowDeletingOfUsedAssets);
        }


        void InitiateDeleteSelectedUsed(BuildInfo buildReportToDisplay)
        {
            BuildReportRemastered.AssetList listToDeleteFrom = GetAssetListToDisplay(buildReportToDisplay);

            InitiateDeleteSelectedInAssetList(buildReportToDisplay, listToDeleteFrom);
        }

        void InitiateDeleteSelectedInAssetList(BuildInfo buildReportToDisplay, BuildReportRemastered.AssetList listToDeleteFrom)
        {
            if (listToDeleteFrom.IsNothingSelected)
            {
                return;
            }


            BuildReportRemastered.SizePart[] all = listToDeleteFrom.All;

            int numOfFilesRequestedToDelete = listToDeleteFrom.GetSelectedCount();
            int numOfFilesToDelete = numOfFilesRequestedToDelete;
            int systemDeletionFileCount = 0;
            int brtFilesSelectedForDelete = 0;


            // filter out files that shouldn't be deleted
            // and identify unrecoverable files
            for (int n = 0, len = all.Length; n < len; ++n)
            {
                BuildReportRemastered.SizePart b = all[n];
                bool isThisFileToBeDeleted = listToDeleteFrom.InSumSelection(b);

                if (isThisFileToBeDeleted)
                {
                    if (BuildReportRemastered.Util.IsFileInBuildReportFolder(b.Name) &&
                        !BuildReportRemastered.Util.IsUselessFile(b.Name))
                    {
                        //Debug.Log("BRT file? " + b.Name);
                        --numOfFilesToDelete;
                        ++brtFilesSelectedForDelete;
                    }
                    else if (BuildReportRemastered.Util.HaveToUseSystemForDelete(b.Name))
                    {
                        ++systemDeletionFileCount;
                    }
                }
            }

            if (numOfFilesToDelete <= 0)
            {
                if (brtFilesSelectedForDelete > 0)
                {
                    EditorApplication.Beep();
                    EditorUtility.DisplayDialog("Can't delete!",
                        "Take note that for safety, Build Report Tool assets themselves will not be included for deletion.",
                        "OK");
                }

                return;
            }


            // prepare warning message for user

            bool deletingSystemFilesOnly = (systemDeletionFileCount == numOfFilesToDelete);
            bool deleteIsRecoverable = !deletingSystemFilesOnly;

            string plural = "";
            if (numOfFilesToDelete > 1)
            {
                plural = "s";
            }

            string message;

            if (numOfFilesRequestedToDelete != numOfFilesToDelete)
            {
                message = "Among " + numOfFilesRequestedToDelete + " file" + plural + " requested to be deleted, only " +
                          numOfFilesToDelete + " will be deleted.";
            }
            else
            {
                message = "This will delete " + numOfFilesToDelete + " asset" + plural + " in your project.";
            }

            // add warning about BRT files that are skipped
            if (brtFilesSelectedForDelete > 0)
            {
                message += "\n\nTake note that for safety, " + brtFilesSelectedForDelete + " file" +
                           ((brtFilesSelectedForDelete > 1) ? "s" : "") +
                           " found to be Build Report Tool assets are not included for deletion.";
            }

            // add warning about unrecoverable files
            if (systemDeletionFileCount > 0)
            {
                if (deletingSystemFilesOnly)
                {
                    message += "\n\nThe deleted file" + plural + " will not be recoverable from the " +
                               BuildReportRemastered.Util.NameOfOSTrashFolder + ", unless you have your own backup.";
                }
                else
                {
                    message += "\n\nAmong the " + numOfFilesToDelete + " file" + plural + " for deletion, " +
                               systemDeletionFileCount + " will not be recoverable from the " +
                               BuildReportRemastered.Util.NameOfOSTrashFolder + ", unless you have your own backup.";
                }

                message +=
                    "\n\nThis is a limitation in Unity and .NET code. To ensure deleting will move the files to the " +
                    BuildReportRemastered.Util.NameOfOSTrashFolder +
                    " instead, delete your files the usual way using your project view.";
            }
            else
            {
                message += "\n\nThe deleted file" + plural + " can be recovered from your " +
                           BuildReportRemastered.Util.NameOfOSTrashFolder + ".";
            }

            message +=
                "\n\nDeleting a large number of files may take a long time as Unity will rebuild the project's Library folder.\n\nProceed with deleting?";

            EditorApplication.Beep();
            if (!EditorUtility.DisplayDialog("Delete?", message, "Yes", "No"))
            {
                return;
            }

            List<BuildReportRemastered.SizePart> allList = new List<BuildReportRemastered.SizePart>(all);
            List<BuildReportRemastered.SizePart> toRemove = new List<BuildReportRemastered.SizePart>(all.Length / 4);

            // finally, delete the files
            int deletedCount = 0;
            for (int n = 0, len = allList.Count; n < len; ++n)
            {
                BuildReportRemastered.SizePart b = allList[n];


                bool okToDelete = BuildReportRemastered.Util.IsUselessFile(b.Name) ||
                                  !BuildReportRemastered.Util.IsFileInBuildReportFolder(b.Name);

                if (listToDeleteFrom.InSumSelection(b) && okToDelete)
                {
                    // delete this

                    if (BuildReportRemastered.Util.ShowFileDeleteProgress(deletedCount, numOfFilesToDelete, b.Name,
                        deleteIsRecoverable))
                    {
                        return;
                    }

                    BuildReportRemastered.Util.DeleteSizePartFile(b);
                    toRemove.Add(b);
                    ++deletedCount;
                }
            }

            EditorUtility.ClearProgressBar();


            // refresh the asset lists
            allList.RemoveAll(i => toRemove.Contains(i));
            BuildReportRemastered.SizePart[] allWithRemoved = allList.ToArray();

            // recreate per category list (maybe just remove from existing per category lists instead?)
            BuildReportRemastered.SizePart[][] perCategoryOfList =
                BuildReportRemastered.ReportGenerator.SegregateAssetSizesPerCategory(allWithRemoved,
                    buildReportToDisplay.FileFilters);

            listToDeleteFrom.Reinit(allWithRemoved, perCategoryOfList,
                IsShowingUsedAssets
                    ? BuildReportRemastered.Options.NumberOfTopLargestUsedAssetsToShow
                    : BuildReportRemastered.Options.NumberOfTopLargestUnusedAssetsToShow);
            listToDeleteFrom.ClearSelection();


            // print info about the delete operation to console
            string finalMessage = string.Format("{0} file{1} removed from your project.", deletedCount.ToString(), plural);
            if (deleteIsRecoverable)
            {
                finalMessage += " They can be recovered from your " + BuildReportRemastered.Util.NameOfOSTrashFolder + ".";
            }

            EditorApplication.Beep();
            EditorUtility.DisplayDialog("Delete successful", finalMessage, "OK");

            Debug.LogWarning(finalMessage);
        }


        void InitiateDeleteAllUnused(BuildInfo buildReportToDisplay)
        {
            BuildReportRemastered.AssetList list = buildReportToDisplay.UnusedAssets;
            BuildReportRemastered.SizePart[] all = list.All;

            int filesToDeleteCount = 0;

            for (int n = 0, len = all.Length; n < len; ++n)
            {
                BuildReportRemastered.SizePart b = all[n];

                bool okToDelete = BuildReportRemastered.Util.IsFileOkForDeleteAllOperation(b.Name);

                if (okToDelete)
                {
                    //Debug.Log("added " + b.Name + " for deletion");
                    ++filesToDeleteCount;
                }
            }

            if (filesToDeleteCount == 0)
            {
                const string NOTHING_TO_DELETE =
                    "Take note that for safety, Build Report Tool assets, Unity editor assets, version control metadata, and Unix-style hidden files will not be included for deletion.\n\nYou can force deleting them by selecting them (via the checkbox) and using \"Delete selected\", or simply delete them the normal way in your project view.";

                EditorApplication.Beep();
                EditorUtility.DisplayDialog("Nothing to delete!", NOTHING_TO_DELETE, "Ok");
                return;
            }

            string plural = "";
            if (filesToDeleteCount > 1)
            {
                plural = "s";
            }

            EditorApplication.Beep();
            if (!EditorUtility.DisplayDialog("Delete?",
                    string.Format(
                        "Among {0} file{1} in your project, {2} will be deleted.\n\nBuild Report Tool assets themselves, Unity editor assets, version control metadata, and Unix-style hidden files will not be included for deletion. You can force-delete those by selecting them (via the checkbox) and use \"Delete selected\", or simply delete them the normal way in your project view.\n\nDeleting a large number of files may take a long time as Unity will rebuild the project's Library folder.\n\nAre you sure about this?\n\nThe file{1} can be recovered from your {3}.",
                        all.Length.ToString(), plural, filesToDeleteCount.ToString(),
                        BuildReportRemastered.Util.NameOfOSTrashFolder), "Yes", "No"))
            {
                return;
            }

            List<BuildReportRemastered.SizePart> newAll = new List<BuildReportRemastered.SizePart>();

            int deletedCount = 0;
            for (int n = 0, len = all.Length; n < len; ++n)
            {
                BuildReportRemastered.SizePart b = all[n];

                bool okToDelete = BuildReportRemastered.Util.IsFileOkForDeleteAllOperation(b.Name);

                if (okToDelete)
                {
                    // delete this
                    if (BuildReportRemastered.Util.ShowFileDeleteProgress(deletedCount, filesToDeleteCount, b.Name, true))
                    {
                        return;
                    }

                    BuildReportRemastered.Util.DeleteSizePartFile(b);
                    ++deletedCount;
                }
                else
                {
                    //Debug.Log("added " + b.Name + " to new list");
                    newAll.Add(b);
                }
            }

            EditorUtility.ClearProgressBar();

            BuildReportRemastered.SizePart[] newAllArr = newAll.ToArray();

            BuildReportRemastered.SizePart[][] perCategoryUnused =
                BuildReportRemastered.ReportGenerator.SegregateAssetSizesPerCategory(newAllArr, buildReportToDisplay.FileFilters);

            list.Reinit(newAllArr, perCategoryUnused,
                IsShowingUsedAssets
                    ? BuildReportRemastered.Options.NumberOfTopLargestUsedAssetsToShow
                    : BuildReportRemastered.Options.NumberOfTopLargestUnusedAssetsToShow);
            list.ClearSelection();


            string finalMessage = string.Format(
                "{0} file{1} removed from your project. They can be recovered from your {2}.",
                filesToDeleteCount.ToString(), plural, BuildReportRemastered.Util.NameOfOSTrashFolder);
            Debug.LogWarning(finalMessage);

            EditorApplication.Beep();
            EditorUtility.DisplayDialog("Delete successful", finalMessage, "OK");
        }
    }
}