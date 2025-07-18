﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.IServices
{
    public interface ICloudinaryService
    {
        Task<string> UploadFileAsync(Stream fileStream, string fileName, string folder);
        Task<string> UploadAudioAsync(Stream fileStream, string fileName, string folder);
    }
}
