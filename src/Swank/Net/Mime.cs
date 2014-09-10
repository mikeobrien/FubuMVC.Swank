namespace FubuMVC.Swank.Net
{
    public enum MimeType
    {
        /// Any type
        Any,

        /// Atom feeds
        ApplicationAtomXml,

        /// Dart files
        ApplicationDart,

        /// ECMAScript/JavaScript; Defined in RFC 4329 (equivalent to application/javascript 
        /// but with stricter processing rules)
        ApplicationEcmascript,

        /// EDI X12 data; Defined in RFC 1767
        ApplicationEdiX12,

        /// EDI EDIFACT data; Defined in RFC 1767
        ApplicationEdifact,

        /// JavaScript Object Notation JSON; Defined in RFC 4627
        ApplicationJson,

        /// ECMAScript/JavaScript; Defined in RFC 4329 (equivalent to application/ecmascript 
        /// but with looser processing rules) It is not accepted in IE 8 or earlier - 
        /// text/javascript is accepted but it is defined as obsolete in RFC 4329. The 
        /// "type" attribute of the <script> tag in HTML5 is optional. In practice, 
        /// omitting the media type of JavaScript programs is the most interoperable 
        /// solution, since all browsers have always assumed the correct default even before HTML5.
        ApplicationJavascript,

        /// Arbitrary binary data. Generally speaking this type identifies files that 
        /// are not associated with a specific application. Contrary to past assumptions 
        /// by software packages such as Apache this is not a type that should be applied 
        /// to unknown files. In such a case, a server or application should not indicate 
        /// a content type, as it may be incorrect, but rather, should omit the type in 
        /// order to allow the recipient to guess the type.
        ApplicationOctetStream,

        /// Ogg, a multimedia bitstream container format; Defined in RFC 5334
        ApplicationOgg,

        /// Portable Document Format, PDF has been in use for document exchange on the 
        /// Internet since 1993; Defined in RFC 3778
        ApplicationPdf,

        /// PostScript; Defined in RFC 2046
        ApplicationPostscript,

        /// Resource Description Framework; Defined by RFC 3870
        ApplicationRdfXml,

        /// RSS feeds
        ApplicationRssXml,

        /// SOAP; Defined by RFC 3902
        ApplicationSoapXml,

        /// Web Open Font Format; (candidate recommendation; use application/x-font-woff 
        /// until standard is official)
        ApplicationFontWoff,

        /// XHTML; Defined by RFC 3236
        ApplicationXhtmlXml,

        /// XML files; Defined by RFC 3023
        ApplicationXml,

        /// DTD files; Defined by RFC 3023
        ApplicationXmlDtd,

        /// XOP
        ApplicationXopXml,

        /// ZIP archive files; Registered
        ApplicationZip,

        /// Gzip, Defined in RFC 6713
        ApplicationGzip,

        /// example in documentation, Defined in RFC 4735
        ApplicationExample,

        /// Native Client web module (supplied via Google Web Store only)
        ApplicationXNacl,

        /// Portable Native Client web module (may supplied by any website as it is safer than x-nacl)
        ApplicationXPnacl,

        /// μ-law audio at 8 kHz, 1 channel; Defined in RFC 2046
        AudioBasic,

        /// 24bit Linear PCM audio at 8–48 kHz, 1-N channels; Defined in RFC 3190
        AudioL24,

        /// MP4 audio
        AudioMp4,

        /// MP3 or other MPEG audio; Defined in RFC 3003
        AudioMpeg,

        /// Ogg Vorbis, Speex, Flac and other audio; Defined in RFC 5334
        AudioOgg,

        /// Opus audio
        AudioOpus,

        /// Vorbis encoded audio; Defined in RFC 5215
        AudioVorbis,

        /// RealAudio; Documented in RealPlayer Help
        AudioVndRnRealaudio,

        /// WAV audio; Defined in RFC 2361
        AudioVndWave,

        /// WebM open media format
        AudioWebm,

        /// example in documentation, Defined in RFC 4735
        AudioExample,

        /// GIF image; Defined in RFC 2045 and RFC 2046
        ImageGif,

        /// JPEG JFIF image; Defined in RFC 2045 and RFC 2046
        ImageJpeg,

        /// JPEG JFIF image; Associated with Internet Explorer; Listed in ms775147(v=vs.85) - 
        /// Progressive JPEG, initiated before global browser support for progressive JPEGs 
        /// (Microsoft and Firefox).
        ImagePjpeg,

        /// Portable Network Graphics; Registered, Defined in RFC 2083
        ImagePng,

        /// SVG vector image; Defined in SVG Tiny 1.2 Specification Appendix M
        ImageSvgXml,

        /// DjVu image and multipage document format.
        ImageVndDjvu,

        /// example in documentation, Defined in RFC 4735
        ImageExample,

        /// Defined in RFC 7230
        MessageHttp,

        /// IMDN Instant Message Disposition Notification; Defined in RFC 5438
        MessageImdnXml,

        /// Email; Defined in RFC 2045 and RFC 2046
        MessagePartial,

        /// Email; EML files, MIME files, MHT files, MHTML files; Defined in RFC 2045 and RFC 2046
        MessageRfc822,

        /// example in documentation, Defined in RFC 4735
        MessageExample,

        /// IGS files, IGES files; Defined in RFC 2077
        ModelIges,

        /// MSH files, MESH files; Defined in RFC 2077, SILO files
        ModelMesh,

        /// WRL files, VRML files; Defined in RFC 2077
        ModelVrml,

        /// X3D ISO standard for representing 3D computer graphics, X3DB binary files -
        ///  never Internet Assigned Numbers Authority approved
        ModelX3dBinary,

        /// X3D ISO standard for representing 3D computer graphics, X3DB binary files 
        /// (application in process, this replaces any use of model/x3d+binary)
        ModelX3dFastinfoset,

        /// X3D ISO standard for representing 3D computer graphics, X3DV VRML files 
        /// (application in process, previous uses may have been model/x3d+vrml)
        ModelX3dVrml,

        /// X3D ISO standard for representing 3D computer graphics, X3D XML files
        ModelX3dXml,

        /// example in documentation, Defined in RFC 4735
        ModelExample,

        /// MIME Email; Defined in RFC 2045 and RFC 2046
        MultipartMixed,

        /// MIME Email; Defined in RFC 2045 and RFC 2046
        MultipartAlternative,

        /// MIME Email; Defined in RFC 2387 and used by MHTML (HTML mail)
        MultipartRelated,

        /// MIME Webform; Defined in RFC 2388
        MultipartFormData,

        /// Defined in RFC 1847
        MultipartSigned,

        /// Defined in RFC 1847
        MultipartEncrypted,

        /// example in documentation, Defined in RFC 4735
        MultipartExample,

        /// commands; subtype resident in Gecko browsers like Firefox 3.5
        TextCmd,

        /// Cascading Style Sheets; Defined in RFC 2318
        TextCss,

        /// Comma-separated values; Defined in RFC 4180
        TextCsv,

        /// example in documentation, Defined in RFC 4735
        TextExample,

        /// HTML; Defined in RFC 2854
        TextHtml,

        /// JavaScript; Defined in and made obsolete in RFC 4329 in order to discourage 
        /// its usage in favor of application/javascript. However, text/javascript is 
        /// allowed in HTML 4 and 5 and, unlike application/javascript, has cross-browser 
        /// support. The "type" attribute of the <script> tag in HTML5 is optional and 
        /// there is no need to use it at all since all browsers have always assumed 
        /// the correct default (even in HTML 4 where it was required by the specification).
        TextJavascript,

        /// Textual data; Defined in RFC 2046 and RFC 3676
        TextPlain,

        /// RTF; Defined by Paul Lindner
        TextRtf,

        /// vCard (contact information); Defined in RFC 6350
        TextVcard,

        /// ABC music notation; Registered
        TextVndAbc,

        /// Extensible Markup Language; Defined in RFC 3023
        TextXml,

        /// Covers most Windows-compatible formats including .avi and .divx
        VideoAvi,

        /// example in documentation, Defined in RFC 4735
        VideoExample,

        /// MPEG-1 video with multiplexed audio; Defined in RFC 2045 and RFC 2046
        VideoMpeg,

        /// MP4 video; Defined in RFC 4337
        VideoMp4,

        /// Ogg Theora or other video (with audio); Defined in RFC 5334
        VideoOgg,

        /// QuickTime video; Registered
        VideoQuicktime,

        /// WebM Matroska-based open media format
        VideoWebm,

        /// Matroska open media format
        VideoXMatroska,

        /// Windows Media Video; Documented in Microsoft KB 288102
        VideoXMsWmv,

        /// Flash video (FLV files)
        VideoXFlv,

        /// deb (file format), a software package format used by the Debian project; Registered
        ApplicationVndDebianBinaryPackage,

        /// Microsoft Excel files
        ApplicationVndMsExcel,

        /// Microsoft Powerpoint files
        ApplicationVndMsPowerpoint,

        /// Mozilla XUL files
        ApplicationVndMozillaXulXml,

        /// KML files (e.g. for Google Earth)
        ApplicationVndGoogleEarthKmlXml,

        /// KMZ files (e.g. for Google Earth)
        ApplicationVndGoogleEarthKmz,

        /// For download apk files.
        ApplicationVndAndroidPackageArchive,

        /// XPS document
        ApplicationVndMsXpsdocument,

        /// 7-Zip compression format.
        ApplicationX7zCompressed,

        /// Google Chrome/Chrome OS extension, app, or theme package
        ApplicationXChromeExtension,

        /// device-independent document in DVI format
        ApplicationXDvi,

        /// TrueType Font No registered MIME type, but this is the most commonly used
        ApplicationXFontTtf,

        /// 
        ApplicationXJavascript,

        /// LaTeX files
        ApplicationXLatex,

        /// .m3u8 variant playlist
        ApplicationXMpegurl,

        /// RAR archive files
        ApplicationXRarCompressed,

        /// Adobe Flash files for example with the extension .swf
        ApplicationXShockwaveFlash,

        /// StuffIt archive files
        ApplicationXStuffit,

        /// Tarball files
        ApplicationXTar,

        /// Form Encoded Data; Documented in HTML 4.01 Specification, Section 17.13.4.1
        ApplicationXWwwFormUrlencoded,

        /// Add-ons to Mozilla applications (Firefox, Thunderbird, SeaMonkey, and the discontinued Sunbird)
        ApplicationXXpinstall,

        /// .aac audio files
        AudioXAac,

        /// Apple's CAF audio files
        AudioXCaf,

        /// GIMP image file
        ImageXXcf,

        /// GoogleWebToolkit data
        TextXGwtRpc,

        /// jQuery template data
        TextXJqueryTmpl,

        /// Markdown formatted text
        TextXMarkdown,

        /// a variant of PKCS standard files
        ApplicationXPkcs12
    }

    public static class Mime
    {
        /// Any type
        public const string Any = "*/*";

        /// Atom feeds
        public const string ApplicationAtomXml = "application/atom+xml";

        /// Dart files
        public const string ApplicationDart = "application/dart";

        /// ECMAScript/JavaScript; Defined in RFC 4329 (equivalent to application/javascript 
        /// but with stricter processing rules)
        public const string ApplicationEcmascript = "application/ecmascript";

        /// EDI X12 data; Defined in RFC 1767
        public const string ApplicationEdiX12 = "application/EDI-X12";

        /// EDI EDIFACT data; Defined in RFC 1767
        public const string ApplicationEdifact = "application/EDIFACT";

        /// JavaScript Object Notation JSON; Defined in RFC 4627
        public const string ApplicationJson = "application/json";

        /// ECMAScript/JavaScript; Defined in RFC 4329 (equivalent to application/ecmascript 
        /// but with looser processing rules) It is not accepted in IE 8 or earlier - 
        /// text/javascript is accepted but it is defined as obsolete in RFC 4329. 
        /// The "type" attribute of the <script> tag in HTML5 is optional. In practice, 
        /// omitting the media type of JavaScript programs is the most interoperable solution, 
        /// since all browsers have always assumed the correct default even before HTML5.
        public const string ApplicationJavascript = "application/javascript";

        /// Arbitrary binary data. Generally speaking this type identifies files that are not 
        /// associated with a specific application. Contrary to past assumptions by software packages 
        /// such as Apache this is not a type that should be applied to unknown files. In such a case, 
        /// a server or application should not indicate a content type, as it may be incorrect, but 
        /// rather, should omit the type in order to allow the recipient to guess the type.
        public const string ApplicationOctetStream = "application/octet-stream";

        /// Ogg, a multimedia bitstream container format; Defined in RFC 5334
        public const string ApplicationOgg = "application/ogg";

        /// Portable Document Format, PDF has been in use for document exchange on the Internet 
        /// since 1993; Defined in RFC 3778
        public const string ApplicationPdf = "application/pdf";

        /// PostScript; Defined in RFC 2046
        public const string ApplicationPostscript = "application/postscript";

        /// Resource Description Framework; Defined by RFC 3870
        public const string ApplicationRdfXml = "application/rdf+xml";

        /// RSS feeds
        public const string ApplicationRssXml = "application/rss+xml";

        /// SOAP; Defined by RFC 3902
        public const string ApplicationSoapXml = "application/soap+xml";

        /// Web Open Font Format; (candidate recommendation; use application/x-font-woff until 
        /// standard is official)
        public const string ApplicationFontWoff = "application/font-woff";

        /// XHTML; Defined by RFC 3236
        public const string ApplicationXhtmlXml = "application/xhtml+xml";

        /// XML files; Defined by RFC 3023
        public const string ApplicationXml = "application/xml";

        /// DTD files; Defined by RFC 3023
        public const string ApplicationXmlDtd = "application/xml-dtd";

        /// XOP
        public const string ApplicationXopXml = "application/xop+xml";

        /// ZIP archive files; Registered
        public const string ApplicationZip = "application/zip";

        /// Gzip, Defined in RFC 6713
        public const string ApplicationGzip = "application/gzip";

        /// example in documentation, Defined in RFC 4735
        public const string ApplicationExample = "application/example";

        /// Native Client web module (supplied via Google Web Store only)
        public const string ApplicationXNacl = "application/x-nacl";

        /// Portable Native Client web module (may supplied by any website as it is safer than x-nacl)
        public const string ApplicationXPnacl = "application/x-pnacl";

        /// μ-law audio at 8 kHz, 1 channel; Defined in RFC 2046
        public const string AudioBasic = "audio/basic";

        /// 24bit Linear PCM audio at 8–48 kHz, 1-N channels; Defined in RFC 3190
        public const string AudioL24 = "audio/L24";

        /// MP4 audio
        public const string AudioMp4 = "audio/mp4";

        /// MP3 or other MPEG audio; Defined in RFC 3003
        public const string AudioMpeg = "audio/mpeg";

        /// Ogg Vorbis, Speex, Flac and other audio; Defined in RFC 5334
        public const string AudioOgg = "audio/ogg";

        /// Opus audio
        public const string AudioOpus = "audio/opus";

        /// Vorbis encoded audio; Defined in RFC 5215
        public const string AudioVorbis = "audio/vorbis";

        /// RealAudio; Documented in RealPlayer Help
        public const string AudioVndRnRealaudio = "audio/vnd.rn-realaudio";

        /// WAV audio; Defined in RFC 2361
        public const string AudioVndWave = "audio/vnd.wave";

        /// WebM open media format
        public const string AudioWebm = "audio/webm";

        /// example in documentation, Defined in RFC 4735
        public const string AudioExample = "audio/example";

        /// GIF image; Defined in RFC 2045 and RFC 2046
        public const string ImageGif = "image/gif";

        /// JPEG JFIF image; Defined in RFC 2045 and RFC 2046
        public const string ImageJpeg = "image/jpeg";

        /// JPEG JFIF image; Associated with Internet Explorer; Listed in ms775147(v=vs.85) - 
        /// Progressive JPEG, initiated before global browser support for progressive JPEGs 
        /// (Microsoft and Firefox).
        public const string ImagePjpeg = "image/pjpeg";

        /// Portable Network Graphics; Registered, Defined in RFC 2083
        public const string ImagePng = "image/png";

        /// SVG vector image; Defined in SVG Tiny 1.2 Specification Appendix M
        public const string ImageSvgXml = "image/svg+xml";

        /// DjVu image and multipage document format.
        public const string ImageVndDjvu = "image/vnd.djvu";

        /// example in documentation, Defined in RFC 4735
        public const string ImageExample = "image/example";

        /// Defined in RFC 7230
        public const string MessageHttp = "message/http";

        /// IMDN Instant Message Disposition Notification; Defined in RFC 5438
        public const string MessageImdnXml = "message/imdn+xml";

        /// Email; Defined in RFC 2045 and RFC 2046
        public const string MessagePartial = "message/partial";

        /// Email; EML files, MIME files, MHT files, MHTML files; Defined in RFC 2045 and RFC 2046
        public const string MessageRfc822 = "message/rfc822";

        /// example in documentation, Defined in RFC 4735
        public const string MessageExample = "message/example";

        /// IGS files, IGES files; Defined in RFC 2077
        public const string ModelIges = "model/iges";

        /// MSH files, MESH files; Defined in RFC 2077, SILO files
        public const string ModelMesh = "model/mesh";

        /// WRL files, VRML files; Defined in RFC 2077
        public const string ModelVrml = "model/vrml";

        /// X3D ISO standard for representing 3D computer graphics, X3DB binary files - 
        /// never Internet Assigned Numbers Authority approved
        public const string ModelX3dBinary = "model/x3d+binary";

        /// X3D ISO standard for representing 3D computer graphics, X3DB binary files 
        /// (application in process, this replaces any use of model/x3d+binary)
        public const string ModelX3dFastinfoset = "model/x3d+fastinfoset";

        /// X3D ISO standard for representing 3D computer graphics, X3DV VRML files 
        /// (application in process, previous uses may have been model/x3d+vrml)
        public const string ModelX3dVrml = "model/x3d-vrml";

        /// X3D ISO standard for representing 3D computer graphics, X3D XML files
        public const string ModelX3dXml = "model/x3d+xml";

        /// example in documentation, Defined in RFC 4735
        public const string ModelExample = "model/example";

        /// MIME Email; Defined in RFC 2045 and RFC 2046
        public const string MultipartMixed = "multipart/mixed";

        /// MIME Email; Defined in RFC 2045 and RFC 2046
        public const string MultipartAlternative = "multipart/alternative";

        /// MIME Email; Defined in RFC 2387 and used by MHTML (HTML mail)
        public const string MultipartRelated = "multipart/related";

        /// MIME Webform; Defined in RFC 2388
        public const string MultipartFormData = "multipart/form-data";

        /// Defined in RFC 1847
        public const string MultipartSigned = "multipart/signed";

        /// Defined in RFC 1847
        public const string MultipartEncrypted = "multipart/encrypted";

        /// example in documentation, Defined in RFC 4735
        public const string MultipartExample = "multipart/example";

        /// commands; subtype resident in Gecko browsers like Firefox 3.5
        public const string TextCmd = "text/cmd";

        /// Cascading Style Sheets; Defined in RFC 2318
        public const string TextCss = "text/css";

        /// Comma-separated values; Defined in RFC 4180
        public const string TextCsv = "text/csv";

        /// example in documentation, Defined in RFC 4735
        public const string TextExample = "text/example";

        /// HTML; Defined in RFC 2854
        public const string TextHtml = "text/html";

        /// JavaScript; Defined in and made obsolete in RFC 4329 in order to discourage 
        /// its usage in favor of application/javascript. However, text/javascript is 
        /// allowed in HTML 4 and 5 and, unlike application/javascript, has cross-browser 
        /// support. The "type" attribute of the <script> tag in HTML5 is optional and 
        /// there is no need to use it at all since all browsers have always assumed the 
        /// correct default (even in HTML 4 where it was required by the specification).
        public const string TextJavascript = "text/javascript (Obsolete)";

        /// Textual data; Defined in RFC 2046 and RFC 3676
        public const string TextPlain = "text/plain";

        /// RTF; Defined by Paul Lindner
        public const string TextRtf = "text/rtf";

        /// vCard (contact information); Defined in RFC 6350
        public const string TextVcard = "text/vcard";

        /// ABC music notation; Registered
        public const string TextVndAbc = "text/vnd.abc";

        /// Extensible Markup Language; Defined in RFC 3023
        public const string TextXml = "text/xml";

        /// Covers most Windows-compatible formats including .avi and .divx
        public const string VideoAvi = "video/avi";

        /// example in documentation, Defined in RFC 4735
        public const string VideoExample = "video/example";

        /// MPEG-1 video with multiplexed audio; Defined in RFC 2045 and RFC 2046
        public const string VideoMpeg = "video/mpeg";

        /// MP4 video; Defined in RFC 4337
        public const string VideoMp4 = "video/mp4";

        /// Ogg Theora or other video (with audio); Defined in RFC 5334
        public const string VideoOgg = "video/ogg";

        /// QuickTime video; Registered
        public const string VideoQuicktime = "video/quicktime";

        /// WebM Matroska-based open media format
        public const string VideoWebm = "video/webm";

        /// Matroska open media format
        public const string VideoXMatroska = "video/x-matroska";

        /// Windows Media Video; Documented in Microsoft KB 288102
        public const string VideoXMsWmv = "video/x-ms-wmv";

        /// Flash video (FLV files)
        public const string VideoXFlv = "video/x-flv";

        /// deb (file format), a software package format used by the Debian project; Registered
        public const string ApplicationVndDebianBinaryPackage = "application/vnd.debian.binary-package";

        /// Microsoft Excel files
        public const string ApplicationVndMsExcel = "application/vnd.ms-excel";

        /// Microsoft Powerpoint files
        public const string ApplicationVndMsPowerpoint = "application/vnd.ms-powerpoint";

        /// Mozilla XUL files
        public const string ApplicationVndMozillaXulXml = "application/vnd.mozilla.xul+xml";

        /// KML files (e.g. for Google Earth)
        public const string ApplicationVndGoogleEarthKmlXml = "application/vnd.google-earth.kml+xml";

        /// KMZ files (e.g. for Google Earth)
        public const string ApplicationVndGoogleEarthKmz = "application/vnd.google-earth.kmz";

        /// For download apk files.
        public const string ApplicationVndAndroidPackageArchive = "application/vnd.android.package-archive";

        /// XPS document
        public const string ApplicationVndMsXpsdocument = "application/vnd.ms-xpsdocument";

        /// 7-Zip compression format.
        public const string ApplicationX7zCompressed = "application/x-7z-compressed";

        /// Google Chrome/Chrome OS extension, app, or theme package
        public const string ApplicationXChromeExtension = "application/x-chrome-extension";

        /// device-independent document in DVI format
        public const string ApplicationXDvi = "application/x-dvi";

        /// TrueType Font No registered MIME type, but this is the most commonly used
        public const string ApplicationXFontTtf = "application/x-font-ttf";

        /// 
        public const string ApplicationXJavascript = "application/x-javascript";

        /// LaTeX files
        public const string ApplicationXLatex = "application/x-latex";

        /// .m3u8 variant playlist
        public const string ApplicationXMpegurl = "application/x-mpegURL";

        /// RAR archive files
        public const string ApplicationXRarCompressed = "application/x-rar-compressed";

        /// Adobe Flash files for example with the extension .swf
        public const string ApplicationXShockwaveFlash = "application/x-shockwave-flash";

        /// StuffIt archive files
        public const string ApplicationXStuffit = "application/x-stuffit";

        /// Tarball files
        public const string ApplicationXTar = "application/x-tar";

        /// Form Encoded Data; Documented in HTML 4.01 Specification, Section 17.13.4.1
        public const string ApplicationXWwwFormUrlencoded = "application/x-www-form-urlencoded";

        /// Add-ons to Mozilla applications (Firefox, Thunderbird, SeaMonkey, and the discontinued Sunbird)
        public const string ApplicationXXpinstall = "application/x-xpinstall";

        /// .aac audio files
        public const string AudioXAac = "audio/x-aac";

        /// Apple's CAF audio files
        public const string AudioXCaf = "audio/x-caf";

        /// GIMP image file
        public const string ImageXXcf = "image/x-xcf";

        /// GoogleWebToolkit data
        public const string TextXGwtRpc = "text/x-gwt-rpc";

        /// jQuery template data
        public const string TextXJqueryTmpl = "text/x-jquery-tmpl";

        /// Markdown formatted text
        public const string TextXMarkdown = "text/x-markdown";

        /// a variant of PKCS standard files
        public const string ApplicationXPkcs12 = "application/x-pkcs12";

        public static string ToMimeType(this MimeType mimeType)
        {
            switch (mimeType)
            {
                case MimeType.Any: return Any;
                case MimeType.ApplicationAtomXml: return ApplicationAtomXml;
                case MimeType.ApplicationDart: return ApplicationDart;
                case MimeType.ApplicationEcmascript: return ApplicationEcmascript;
                case MimeType.ApplicationEdiX12: return ApplicationEdiX12;
                case MimeType.ApplicationEdifact: return ApplicationEdifact;
                case MimeType.ApplicationJson: return ApplicationJson;
                case MimeType.ApplicationJavascript: return ApplicationJavascript;
                case MimeType.ApplicationOctetStream: return ApplicationOctetStream;
                case MimeType.ApplicationOgg: return ApplicationOgg;
                case MimeType.ApplicationPdf: return ApplicationPdf;
                case MimeType.ApplicationPostscript: return ApplicationPostscript;
                case MimeType.ApplicationRdfXml: return ApplicationRdfXml;
                case MimeType.ApplicationRssXml: return ApplicationRssXml;
                case MimeType.ApplicationSoapXml: return ApplicationSoapXml;
                case MimeType.ApplicationFontWoff: return ApplicationFontWoff;
                case MimeType.ApplicationXhtmlXml: return ApplicationXhtmlXml;
                case MimeType.ApplicationXml: return ApplicationXml;
                case MimeType.ApplicationXmlDtd: return ApplicationXmlDtd;
                case MimeType.ApplicationXopXml: return ApplicationXopXml;
                case MimeType.ApplicationZip: return ApplicationZip;
                case MimeType.ApplicationGzip: return ApplicationGzip;
                case MimeType.ApplicationExample: return ApplicationExample;
                case MimeType.ApplicationXNacl: return ApplicationXNacl;
                case MimeType.ApplicationXPnacl: return ApplicationXPnacl;
                case MimeType.AudioBasic: return AudioBasic;
                case MimeType.AudioL24: return AudioL24;
                case MimeType.AudioMp4: return AudioMp4;
                case MimeType.AudioMpeg: return AudioMpeg;
                case MimeType.AudioOgg: return AudioOgg;
                case MimeType.AudioOpus: return AudioOpus;
                case MimeType.AudioVorbis: return AudioVorbis;
                case MimeType.AudioVndRnRealaudio: return AudioVndRnRealaudio;
                case MimeType.AudioVndWave: return AudioVndWave;
                case MimeType.AudioWebm: return AudioWebm;
                case MimeType.AudioExample: return AudioExample;
                case MimeType.ImageGif: return ImageGif;
                case MimeType.ImageJpeg: return ImageJpeg;
                case MimeType.ImagePjpeg: return ImagePjpeg;
                case MimeType.ImagePng: return ImagePng;
                case MimeType.ImageSvgXml: return ImageSvgXml;
                case MimeType.ImageVndDjvu: return ImageVndDjvu;
                case MimeType.ImageExample: return ImageExample;
                case MimeType.MessageHttp: return MessageHttp;
                case MimeType.MessageImdnXml: return MessageImdnXml;
                case MimeType.MessagePartial: return MessagePartial;
                case MimeType.MessageRfc822: return MessageRfc822;
                case MimeType.MessageExample: return MessageExample;
                case MimeType.ModelIges: return ModelIges;
                case MimeType.ModelMesh: return ModelMesh;
                case MimeType.ModelVrml: return ModelVrml;
                case MimeType.ModelX3dBinary: return ModelX3dBinary;
                case MimeType.ModelX3dFastinfoset: return ModelX3dFastinfoset;
                case MimeType.ModelX3dVrml: return ModelX3dVrml;
                case MimeType.ModelX3dXml: return ModelX3dXml;
                case MimeType.ModelExample: return ModelExample;
                case MimeType.MultipartMixed: return MultipartMixed;
                case MimeType.MultipartAlternative: return MultipartAlternative;
                case MimeType.MultipartRelated: return MultipartRelated;
                case MimeType.MultipartFormData: return MultipartFormData;
                case MimeType.MultipartSigned: return MultipartSigned;
                case MimeType.MultipartEncrypted: return MultipartEncrypted;
                case MimeType.MultipartExample: return MultipartExample;
                case MimeType.TextCmd: return TextCmd;
                case MimeType.TextCss: return TextCss;
                case MimeType.TextCsv: return TextCsv;
                case MimeType.TextExample: return TextExample;
                case MimeType.TextHtml: return TextHtml;
                case MimeType.TextJavascript: return TextJavascript;
                case MimeType.TextPlain: return TextPlain;
                case MimeType.TextRtf: return TextRtf;
                case MimeType.TextVcard: return TextVcard;
                case MimeType.TextVndAbc: return TextVndAbc;
                case MimeType.TextXml: return TextXml;
                case MimeType.VideoAvi: return VideoAvi;
                case MimeType.VideoExample: return VideoExample;
                case MimeType.VideoMpeg: return VideoMpeg;
                case MimeType.VideoMp4: return VideoMp4;
                case MimeType.VideoOgg: return VideoOgg;
                case MimeType.VideoQuicktime: return VideoQuicktime;
                case MimeType.VideoWebm: return VideoWebm;
                case MimeType.VideoXMatroska: return VideoXMatroska;
                case MimeType.VideoXMsWmv: return VideoXMsWmv;
                case MimeType.VideoXFlv: return VideoXFlv;
                case MimeType.ApplicationVndDebianBinaryPackage: return ApplicationVndDebianBinaryPackage;
                case MimeType.ApplicationVndMsExcel: return ApplicationVndMsExcel;
                case MimeType.ApplicationVndMsPowerpoint: return ApplicationVndMsPowerpoint;
                case MimeType.ApplicationVndMozillaXulXml: return ApplicationVndMozillaXulXml;
                case MimeType.ApplicationVndGoogleEarthKmlXml: return ApplicationVndGoogleEarthKmlXml;
                case MimeType.ApplicationVndGoogleEarthKmz: return ApplicationVndGoogleEarthKmz;
                case MimeType.ApplicationVndAndroidPackageArchive: return ApplicationVndAndroidPackageArchive;
                case MimeType.ApplicationVndMsXpsdocument: return ApplicationVndMsXpsdocument;
                case MimeType.ApplicationX7zCompressed: return ApplicationX7zCompressed;
                case MimeType.ApplicationXChromeExtension: return ApplicationXChromeExtension;
                case MimeType.ApplicationXDvi: return ApplicationXDvi;
                case MimeType.ApplicationXFontTtf: return ApplicationXFontTtf;
                case MimeType.ApplicationXJavascript: return ApplicationXJavascript;
                case MimeType.ApplicationXLatex: return ApplicationXLatex;
                case MimeType.ApplicationXMpegurl: return ApplicationXMpegurl;
                case MimeType.ApplicationXRarCompressed: return ApplicationXRarCompressed;
                case MimeType.ApplicationXShockwaveFlash: return ApplicationXShockwaveFlash;
                case MimeType.ApplicationXStuffit: return ApplicationXStuffit;
                case MimeType.ApplicationXTar: return ApplicationXTar;
                case MimeType.ApplicationXWwwFormUrlencoded: return ApplicationXWwwFormUrlencoded;
                case MimeType.ApplicationXXpinstall: return ApplicationXXpinstall;
                case MimeType.AudioXAac: return AudioXAac;
                case MimeType.AudioXCaf: return AudioXCaf;
                case MimeType.ImageXXcf: return ImageXXcf;
                case MimeType.TextXGwtRpc: return TextXGwtRpc;
                case MimeType.TextXJqueryTmpl: return TextXJqueryTmpl;
                case MimeType.TextXMarkdown: return TextXMarkdown;
                case MimeType.ApplicationXPkcs12: return ApplicationXPkcs12;
                default: return null;
            }
        }
    }
}