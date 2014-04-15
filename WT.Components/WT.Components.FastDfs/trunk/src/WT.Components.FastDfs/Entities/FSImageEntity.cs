using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace WT.Components.FastDfs.Entities
{
   /// <summary>
   /// 图片文件
   /// </summary>
    public class FSImageEntity
    {
        public Image SourcePic { get; set; }
        public MemoryStream JpegStream { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public byte[] JpegBytes{ get; set; }

        private Dictionary<string, ImageCodecInfo> encodecrs;
        public Dictionary<string, ImageCodecInfo> Encodecrs
        {
            get {
                if (encodecrs == null)
                {
                    encodecrs = new Dictionary<string, ImageCodecInfo>();
                }

                if (encodecrs.Count == 0)
                {
                    foreach (ImageCodecInfo codec in ImageCodecInfo.GetImageEncoders())
                    {
                        encodecrs.Add(codec.MimeType.ToLower(), codec);
                    }
                }

                return encodecrs;
            }
        }

        public FSImageEntity(Image img, float fwidth, float fheight)
        {
            float rate = (fwidth / img.Size.Width > fheight / img.Size.Height) ?
                        (float)(fheight / img.Size.Height) : (float)(fwidth / img.Size.Width);

            SourcePic = img;
            Width = (int)Math.Ceiling(img.Size.Width * rate);
            Height = (int)Math.Ceiling(img.Size.Height * rate);
            JpegStream = null;
            
        }

        /// <summary>
        /// 转换文件尺寸
        /// </summary>
        public void ConvertSourcePic()
        {
            if (validImageType(SourcePic))
                InitJpegSrteam(ResizeImage(SourcePic, Width, Height), 85);
        }

        /// <summary>
        /// 验证图片文件有效性
        /// </summary>
        /// <param name="img"></param>
        /// <returns></returns>
        public bool validImageType(Image img)
        {
            ImageFormat format = img.RawFormat;
            ImageCodecInfo codec = ImageCodecInfo.GetImageDecoders().First(c => c.FormatID == format.Guid);
            string mimeType = codec.MimeType;

            ImageCodecInfo codecInfo = GetEncodecInfo(mimeType);

            return (codecInfo == null) ? false : true;
        }

        /// <summary>
        /// 重置并获取指定尺寸图片
        /// </summary>
        /// <param name="img"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        private Bitmap ResizeImage(Image img, int width, int height)
        {
            Bitmap result = new Bitmap(width, height);
            result.SetResolution(img.HorizontalResolution, img.VerticalResolution);

            using (Graphics graphics = Graphics.FromImage(result))
            {
                //set the resize quality modes to high quality
                graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                //draw the image into the target bitmap
                graphics.DrawImage(img, 0, 0, result.Width, result.Height);
            }

            return result;
        }

        /// <summary>
        /// 获取jpeg格式文件字节流
        /// </summary>
        /// <param name="img"></param>
        /// <param name="quality"></param>
        /// <returns></returns>
        private void InitJpegSrteam(Image img, int quality )
        {
            quality = ((quality < 0) || (quality > 100)) ? 100 : quality;

            EncoderParameter qualityParm = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);
            ImageCodecInfo jpegCodec = GetEncodecInfo("image/jpeg");

            EncoderParameters encoderParams = new EncoderParameters(1);
            encoderParams.Param[0] = qualityParm;

            using (JpegStream = new MemoryStream())
            {
                img.Save(JpegStream, jpegCodec, encoderParams);
                JpegBytes = new byte[JpegStream.Length];
                JpegStream.Seek(0, SeekOrigin.Begin);
                JpegStream.Read(JpegBytes, 0, JpegBytes.Length);
            }

        }

        private ImageCodecInfo GetEncodecInfo(string mimetype)
        {
            string lookupkey = mimetype.ToLower();
            ImageCodecInfo codecInfo = null;

            if (Encodecrs.ContainsKey(lookupkey))
            {
                codecInfo = Encodecrs[lookupkey];
            }

            return codecInfo;
        }
    }
}
