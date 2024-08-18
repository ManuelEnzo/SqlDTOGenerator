using Avalonia.Controls;
using Avalonia.Platform.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlDTOGeneratorDesktopApp.Services
{
    public interface IFileService
    {
        public Task<IStorageFile?> OpenFileAsync();
        public Task<IStorageFile?> SaveFileAsync();
        public Task<IReadOnlyList<IStorageFolder>> OpenFolderAsync();
    }
    public class FilesService : IFileService
    {
        private readonly Window _target;

        public FilesService(Window target)
        {
            _target = target;
        }

        public async Task<IStorageFile?> OpenFileAsync()
        {
            var files = await _target.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions()
            {
                Title = "Open Text File",
                AllowMultiple = false
            });

            return files.Count >= 1 ? files[0] : null;
        }

        public async Task<IReadOnlyList<IStorageFolder>> OpenFolderAsync()
        {
            // Apri il selettore di cartelle
            var folders = await _target.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
            {
                AllowMultiple = true,
                Title = "Select Folder Output"
            });

            // Restituisci le cartelle selezionate
            return folders;
        }

        public async Task<IStorageFile?> SaveFileAsync()
        {
            return await _target.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions()
            {
                Title = "Save Text File"
            });
        }
    }
}
