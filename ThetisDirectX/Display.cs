using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Threading;

using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
// fix clashes with sharpdx
using Bitmap = System.Drawing.Bitmap;
using Rectangle = System.Drawing.Rectangle;
using Color = System.Drawing.Color;
using Brush = System.Drawing.Brush;
using Point = System.Drawing.Point;
using Pen = System.Drawing.Pen;
using PixelFormat = System.Drawing.Imaging.PixelFormat;
using DashStyle = System.Drawing.Drawing2D.DashStyle;
// SharpDX clashes
using AlphaMode = SharpDX.Direct2D1.AlphaMode;
using Device = SharpDX.Direct3D11.Device;
using RectangleF = SharpDX.RectangleF;
using SDXPixelFormat = SharpDX.Direct2D1.PixelFormat;

namespace ThetisDirectX
{
    public class Display
    {
        private static readonly Object _objDX2Lock = new Object();
        //
        private static Surface _surface;
        private static SwapChain _swapChain;
        private static SwapChain1 _swapChain1;
        private static RenderTarget _d2dRenderTarget;
        private static SharpDX.Direct2D1.Factory _d2dFactory;
        private static Device _device;
        private static SharpDX.DXGI.Factory _factory1;
        //
        private static bool _bDX2Setup = false;
        private static PresentFlags _NoVSYNCpresentFlag = PresentFlags.None;
        private static bool _bAntiAlias = false;
        private static int _nVBlanks = 0;
        private static Vector2 _pixelShift = new Vector2(0.5f, 0.5f);
        private static int _nBufferCount = 1;
        private static bool _bUseLegacyBuffers = false;

        public static void ShutdownDX2D()
        {
            lock (_objDX2Lock)
            {
                if (!_bDX2Setup) return;

                try
                {
                    if (_device != null && _device.ImmediateContext != null)
                    {
                        _device.ImmediateContext.ClearState();
                        _device.ImmediateContext.Flush();
                    }

                    releaseFonts();
                    releaseDX2Resources();

                    Utilities.Dispose(ref _d2dRenderTarget);
                    Utilities.Dispose(ref _swapChain1);
                    Utilities.Dispose(ref _swapChain);
                    Utilities.Dispose(ref _surface);
                    Utilities.Dispose(ref _d2dFactory);
                    Utilities.Dispose(ref _factory1);

                    _d2dRenderTarget = null;
                    _swapChain1 = null;
                    _swapChain = null;
                    _surface = null;
                    _d2dFactory = null;
                    _factory1 = null;

                    if (_device != null && _device.ImmediateContext != null)
                    {
                        SharpDX.Direct3D11.DeviceContext dc = _device.ImmediateContext;
                        Utilities.Dispose(ref dc);
                        dc = null;
                    }

                    SharpDX.Direct3D11.DeviceDebug ddb = null;
                    if (_device != null && _device.DebugName != "")
                    {
                        ddb = new SharpDX.Direct3D11.DeviceDebug(_device);
                        ddb.ReportLiveDeviceObjects(ReportingLevel.Detail);
                    }

                    if (ddb != null)
                    {
                        Utilities.Dispose(ref ddb);
                        ddb = null;
                    }
                    Utilities.Dispose(ref _device);
                    _device = null;

                    _bDX2Setup = false;
                }
                catch (Exception e)
                {
                    MessageBox.Show("Problem Shutting Down DirectX !" + System.Environment.NewLine + System.Environment.NewLine + "[" + e.ToString() + "]", "DirectX", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
        }

        private static Adapter m_adapter = null;
        public static void InitDX2D(Adapter adapter = null)
        {
            lock (_objDX2Lock)
            {
                if (_bDX2Setup || displayTarget == null) return;

                try
                {
                    m_adapter = adapter;
                    DeviceCreationFlags debug = DeviceCreationFlags.Debug;

                    // to get this to work, need to target the os
                    // https://www.prugg.at/2019/09/09/properly-detect-windows-version-in-c-net-even-windows-10/
                    // you need to enable operating system support in the app1.manifest file, otherwise majVers will not report 10+
                    // note: windows 10, 11, server 2016, server 2019, server 2022 all use the windows 10 os id in the manifest file at this current time
                    int majVers = Environment.OSVersion.Version.Major;
                    int minVers = Environment.OSVersion.Version.Minor;

                    SharpDX.Direct3D.FeatureLevel[] featureLevels;

                    if (majVers >= 10) // win10 + 11
                    {
                        featureLevels = new SharpDX.Direct3D.FeatureLevel[] {
                            SharpDX.Direct3D.FeatureLevel.Level_12_1,
                            SharpDX.Direct3D.FeatureLevel.Level_12_0,
                            SharpDX.Direct3D.FeatureLevel.Level_11_1, // windows 8 and up
                            SharpDX.Direct3D.FeatureLevel.Level_11_0, // windows 7 and up (level 11 was only partial on 7, not 11_1)
                            SharpDX.Direct3D.FeatureLevel.Level_10_1,
                            SharpDX.Direct3D.FeatureLevel.Level_10_0,
                            SharpDX.Direct3D.FeatureLevel.Level_9_3,
                            SharpDX.Direct3D.FeatureLevel.Level_9_2,
                            SharpDX.Direct3D.FeatureLevel.Level_9_1
                        };
                        _NoVSYNCpresentFlag = PresentFlags.DoNotWait;
                    }
                    else if (majVers == 6 && minVers >= 2) // windows 8, windows 8.1
                    {
                        featureLevels = new SharpDX.Direct3D.FeatureLevel[] {
                            SharpDX.Direct3D.FeatureLevel.Level_11_1, // windows 8 and up
                            SharpDX.Direct3D.FeatureLevel.Level_11_0, // windows 7 and up (level 11 was only partial on 7, not 11_1)
                            SharpDX.Direct3D.FeatureLevel.Level_10_1,
                            SharpDX.Direct3D.FeatureLevel.Level_10_0,
                            SharpDX.Direct3D.FeatureLevel.Level_9_3,
                            SharpDX.Direct3D.FeatureLevel.Level_9_2,
                            SharpDX.Direct3D.FeatureLevel.Level_9_1
                        };
                        _NoVSYNCpresentFlag = PresentFlags.DoNotWait;
                    }
                    else if (majVers == 6 && minVers < 2) // windows 7, 2008 R2, 2008, vista
                    {
                        featureLevels = new SharpDX.Direct3D.FeatureLevel[] {
                            SharpDX.Direct3D.FeatureLevel.Level_11_0, // windows 7 and up (level 11 was only partial on 7, not 11_1)
                            SharpDX.Direct3D.FeatureLevel.Level_10_1,
                            SharpDX.Direct3D.FeatureLevel.Level_10_0,
                            SharpDX.Direct3D.FeatureLevel.Level_9_3,
                            SharpDX.Direct3D.FeatureLevel.Level_9_2,
                            SharpDX.Direct3D.FeatureLevel.Level_9_1
                        };
                        _NoVSYNCpresentFlag = PresentFlags.None;
                    }
                    else
                    {
                        featureLevels = new SharpDX.Direct3D.FeatureLevel[] {
                            SharpDX.Direct3D.FeatureLevel.Level_9_1
                        };
                        _NoVSYNCpresentFlag = PresentFlags.None;
                    }

                    _factory1 = new SharpDX.DXGI.Factory1();

                    if (m_adapter != null)
                        _device = new Device(m_adapter, debug | DeviceCreationFlags.PreventAlteringLayerSettingsFromRegistry | DeviceCreationFlags.BgraSupport | DeviceCreationFlags.SingleThreaded, featureLevels);
                    else
                        _device = new Device(DriverType.Hardware, debug | DeviceCreationFlags.PreventAlteringLayerSettingsFromRegistry | DeviceCreationFlags.BgraSupport | DeviceCreationFlags.SingleThreaded, featureLevels);

                    SharpDX.DXGI.Device1 device1 = _device.QueryInterfaceOrNull<SharpDX.DXGI.Device1>();
                    if (device1 != null)
                    {
                        device1.MaximumFrameLatency = 1;
                        Utilities.Dispose(ref device1);
                    }

                    ////this code should ideally be used to prevent use of flip if vsync is 0
                    //SharpDX.DXGI.Factory5 f5 = factory.QueryInterfaceOrNull<SharpDX.DXGI.Factory5>();
                    //bool bAllowTearing = false;
                    //if(f5 != null)
                    //{
                    //    int size = Marshal.SizeOf(typeof(bool));
                    //    IntPtr pBool = Marshal.AllocHGlobal(size);

                    //    f5.CheckFeatureSupport(SharpDX.DXGI.Feature.PresentAllowTearing, pBool, size);

                    //    bAllowTearing = Marshal.ReadInt32(pBool) == 1;

                    //    Marshal.FreeHGlobal(pBool);
                    //}
                    ////

                    // check if the device has a factory4 interface
                    // if not, then we need to use old bitplit swapeffect
                    SwapEffect swapEffect;

                    SharpDX.DXGI.Factory4 factory4 = _factory1.QueryInterfaceOrNull<SharpDX.DXGI.Factory4>();
                    bool bFlipPresent = false;
                    if (factory4 != null)
                    {
                        if (!_bUseLegacyBuffers) bFlipPresent = true;
                        Utilities.Dispose(ref factory4);
                        factory4 = null;
                    }

                    //https://walbourn.github.io/care-and-feeding-of-modern-swapchains/
                    swapEffect = bFlipPresent ? SwapEffect.FlipDiscard : SwapEffect.Discard; //NOTE: FlipSequential should work, but is mostly used for storeapps
                    _nBufferCount = bFlipPresent ? 2 : 1;

                    //int multiSample = 8; // eg 2 = MSAA_2, 2 times multisampling
                    //int maxQuality = device.CheckMultisampleQualityLevels(Format.B8G8R8A8_UNorm, multiSample) - 1; 
                    //maxQuality = Math.Max(0, maxQuality);

                    ModeDescription md = new ModeDescription(displayTarget.Width, displayTarget.Height,
                                                               new Rational(m_nFps, 1), Format.B8G8R8A8_UNorm);//D2D1_ALPHA_MODE_PREMULTIPLIED
                    md.ScanlineOrdering = DisplayModeScanlineOrder.Progressive;
                    md.Scaling = DisplayModeScaling.Centered;

                    SwapChainDescription desc = new SwapChainDescription()
                    {
                        BufferCount = _nBufferCount,
                        ModeDescription = md,
                        IsWindowed = true,
                        OutputHandle = displayTarget.Handle,
                        //SampleDescription = new SampleDescription(multiSample, maxQuality),
                        SampleDescription = new SampleDescription(1, 0), // no multi sampling (1 sample), no antialiasing
                        SwapEffect = swapEffect,
                        Usage = Usage.RenderTargetOutput,// | Usage.BackBuffer,  // dont need usage.backbuffer as it is implied
                        Flags = SwapChainFlags.None,
                    };

                    _factory1.MakeWindowAssociation(displayTarget.Handle, WindowAssociationFlags.IgnoreAll);

                    _swapChain = new SwapChain(_factory1, _device, desc);
                    _swapChain1 = _swapChain.QueryInterface<SwapChain1>();

                    _d2dFactory = new SharpDX.Direct2D1.Factory(FactoryType.SingleThreaded, DebugLevel.None);

                    _surface = _swapChain1.GetBackBuffer<Surface>(0);

                    RenderTargetProperties rtp = new RenderTargetProperties(new SDXPixelFormat(Format.B8G8R8A8_UNorm, AlphaMode.Premultiplied));
                    _d2dRenderTarget = new RenderTarget(_d2dFactory, _surface, rtp);

                    if (debug == DeviceCreationFlags.Debug)
                    {
                        _device.DebugName = "DeviceDB";
                        _swapChain.DebugName = "SwapChainDB";
                        _swapChain1.DebugName = "SwapChain1DB";
                        _surface.DebugName = "SurfaceDB";
                    }
                    else
                    {
                        _device.DebugName = ""; // used in shutdown
                    }

                    _bDX2Setup = true;

                    setupAliasing();

                    buildDX2Resources();
                    buildFontsDX2D();
                }
                catch (Exception e)
                {
                    // issue setting up dx
                    ShutdownDX2D();
                    MessageBox.Show("Problem initialising DirectX !" + System.Environment.NewLine + System.Environment.NewLine + "[" + e.ToString() + "]", "DirectX", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
        }

        private static void setupAliasing()
        {
            lock (_objDX2Lock)
            {
                if (!_bDX2Setup) return;

                if (_bAntiAlias)
                    _d2dRenderTarget.AntialiasMode = AntialiasMode.PerPrimitive; // this will antialias even if multisampling is off
                else
                    _d2dRenderTarget.AntialiasMode = AntialiasMode.Aliased; // this will result in non antialiased lines only if multisampling = 1

                _d2dRenderTarget.TextAntialiasMode = TextAntialiasMode.Default;
            }
        }

        public static void ResizeDX2D()
        {
            try
            {
                lock (_objDX2Lock)
                {
                    if (!_bDX2Setup) return;

                    if (_device != null && _device.ImmediateContext != null)
                    {
                        _device.ImmediateContext.ClearState();
                        _device.ImmediateContext.Flush();
                    }

                    Utilities.Dispose(ref _d2dRenderTarget);
                    Utilities.Dispose(ref _surface);

                    _d2dRenderTarget = null;
                    _surface = null;

                    _swapChain1.ResizeBuffers(_nBufferCount, displayTargetWidth, displayTargetHeight, Format.B8G8R8A8_UNorm, SwapChainFlags.None);

                    _surface = _swapChain1.GetBackBuffer<Surface>(0);

                    RenderTargetProperties rtp = new RenderTargetProperties(new SDXPixelFormat(Format.B8G8R8A8_UNorm, AlphaMode.Premultiplied));
                    _d2dRenderTarget = new RenderTarget(_d2dFactory, _surface, rtp);

                    setupAliasing();
                }
            }
            catch (Exception e)
            {
                ShutdownDX2D();
                MessageBox.Show("DirectX ResizeDX2D() failure\n" + e.Message, "DirectX", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private static SharpDX.DirectWrite.Factory m_fontFactory;
        private static SharpDX.DirectWrite.TextFormat m_textFormFormat;
        //
        private static SharpDX.Direct2D1.Brush m_bDX2textBrush;
        private static SharpDX.Direct2D1.Brush m_DX2redBrush;
        private static SharpDX.Direct2D1.Brush m_DX2lineBrush;
        //
        private static Color m_textColour = Color.Chartreuse;
        private static Font m_textFont = new System.Drawing.Font("Trebuchet MS", 18, FontStyle.Regular);
        private static Brush m_textBrush = new SolidBrush(m_textColour);
        private static Color m_lineColour = Color.FromArgb(255, Color.Teal);
        private static Pen m_lineBrush = new Pen(m_lineColour);
        //
        private static void releaseFonts()
        {
            if (m_fontFactory != null) Utilities.Dispose(ref m_fontFactory);
            if (m_textFormFormat != null) Utilities.Dispose(ref m_textFormFormat);
        }
        private static void releaseDX2Resources()
        {
            if (m_DX2redBrush != null) Utilities.Dispose(ref m_DX2redBrush);
            if (m_bDX2textBrush != null) Utilities.Dispose(ref m_bDX2textBrush);
            //
            if (m_DX2lineBrush != null) Utilities.Dispose(ref m_DX2lineBrush);
        }
        private static Color4 convertColour(Color c)
        {
            return new Color4(c.R / 255f, c.G / 255f, c.B / 255f, c.A / 255f);
        }
        private static SharpDX.Direct2D1.SolidColorBrush convertBrush(SolidBrush b)
        {
            return new SharpDX.Direct2D1.SolidColorBrush(_d2dRenderTarget, convertColour(b.Color));
        }
        private static void buildFontsDX2D()
        {
            lock (_objDX2Lock)
            {
                if (!_bDX2Setup) return;

                releaseFonts();

                m_fontFactory = new SharpDX.DirectWrite.Factory();
                m_textFormFormat = new SharpDX.DirectWrite.TextFormat(m_fontFactory, m_textFont.FontFamily.Name, (m_textFont.Size / 72) * _d2dRenderTarget.DotsPerInch.Width);
            }
        }
        private static void buildDX2Resources()
        {
            lock (_objDX2Lock)
            {
                if (!_bDX2Setup) return;

                releaseDX2Resources();

                m_DX2redBrush = convertBrush(new SolidBrush(Color.Red));
                m_bDX2textBrush = convertBrush((SolidBrush)m_textBrush);
                m_DX2lineBrush = convertBrush((SolidBrush)m_lineBrush.Brush);
            }
        }
        //--------------

        private static int displayTargetHeight = 0;
        private static int displayTargetWidth = 0;
        private static Control displayTarget = null;
        private static int m_nFps = 0;
        private static int m_nFrameCount = 0;
        private static HiPerfTimer m_objFrameStartTimer = new HiPerfTimer();
        private static double m_fLastTime = m_objFrameStartTimer.ElapsedMsec;
        private static double m_dElapsedFrameStart = m_objFrameStartTimer.ElapsedMsec;


        public static Control Target
        {
            get { return displayTarget; }
            set
            {
                lock (_objDX2Lock)
                {
                    displayTarget = value;

                    displayTargetHeight = displayTarget.Height;
                    displayTargetWidth = displayTarget.Width;
                }
            }
        }
        public static int VerticalBlanks
        {
            get { return _nVBlanks; }
            set
            {
                int v = value;
                if (v < 0) v = 0;
                if (v > 4) v = 4;
                _nVBlanks = v;
            }
        }
        public static Adapter DX2Adaptor(string desc)
        {
            SharpDX.DXGI.Factory factory1 = new SharpDX.DXGI.Factory1();

            int nAdaptorCount = factory1.GetAdapterCount();
            //string[] adaptors = new string[nAdaptorCount];

            for (int n = 0; n < nAdaptorCount; n++)
            {
                Adapter adapter = factory1.GetAdapter(n);
                if(adapter.Description.Description == desc)
                {
                    return adapter;
                }
            }

            return null;
        }
        public static string[] DX2Adaptors()
        {
            SharpDX.DXGI.Factory factory1 = new SharpDX.DXGI.Factory1();

            int nAdaptorCount = factory1.GetAdapterCount();
            string[] adaptors = new string[nAdaptorCount];

            for (int n = 0; n < nAdaptorCount; n++)
            {
                Adapter adapter = factory1.GetAdapter(n);
                adaptors[n] = adapter.Description.Description;
                //Utilities.Dispose(ref adapter);
            }

            return adaptors;
        }
        public static int RenderDX2D()
        {
            int nRet = 0;
            try
            {
                lock (_objDX2Lock)
                {
                    if (!_bDX2Setup) return 0; // moved inside the lock so that a change in state by shutdown becomes thread safe

                    m_dElapsedFrameStart = m_objFrameStartTimer.ElapsedMsec;

                    _d2dRenderTarget.BeginDraw();

                    // middle pixel align shift, NOTE: waterfall will switch internally to identity, and then restore
                    Matrix3x2 t = _d2dRenderTarget.Transform;
                    t.TranslationVector = _pixelShift;
                    _d2dRenderTarget.Transform = t;

                    _d2dRenderTarget.Clear(SharpDX.Color.Black);

                    // render here
                    nRet = drawLines();
                    //

                    if (m_bShowFrameRateIssue && m_bFrameRateIssue) _d2dRenderTarget.FillRectangle(new RectangleF(0, 0, 8, 8), m_DX2redBrush);
                    if (m_bShowFPS) _d2dRenderTarget.DrawText(m_nFps.ToString() + " fps", m_textFormFormat, new RectangleF(10, 0, float.PositiveInfinity, float.PositiveInfinity), m_bDX2textBrush, DrawTextOptions.None);

                    // some debug text
                    if (m_sDebugText != "")
                    {
                        string[] lines = m_sDebugText.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
                        int xStartX = 32;
                        for (int i = 0; i < lines.Length; i++)
                        {
                            _d2dRenderTarget.DrawText(lines[i].ToString(), m_textFormFormat, new RectangleF(64, xStartX, float.PositiveInfinity, float.PositiveInfinity), m_bDX2textBrush, DrawTextOptions.None);
                            xStartX += 12;
                        }
                    }

                    // undo the translate
                    _d2dRenderTarget.Transform = Matrix3x2.Identity;

                    _d2dRenderTarget.EndDraw();

                    // render
                    // note: the only way to have Present non block when using vsync number of blanks 0 , is to use DoNotWait
                    // however the gpu will error if it is busy doing something and the data can not be queued
                    // It will error and just ignore everything, we try present and ignore the 0x887A000A error
                    PresentFlags pf = _nVBlanks == 0 ? _NoVSYNCpresentFlag : PresentFlags.None;
                    Result r = _swapChain1.TryPresent(_nVBlanks, pf);

                    if (r != Result.Ok && r != 0x887A000A)
                    {
                        string sMsg = "";
                        if (r == 0x887A0001) sMsg = "Present Device Invalid Call" + Environment.NewLine + "" + Environment.NewLine + "[ " + r.ToString() + " ]";    //DXGI_ERROR_INVALID_CALL
                        if (r == 0x887A0007) sMsg = "Present Device Reset" + Environment.NewLine + "" + Environment.NewLine + "[ " + r.ToString() + " ]";           //DXGI_ERROR_DEVICE_RESET
                        if (r == 0x887A0005) sMsg = "Present Device Removed" + Environment.NewLine + "" + Environment.NewLine + "[ " + r.ToString() + " ]";         //DXGI_ERROR_DEVICE_REMOVED
                        if (r == 0x88760870) sMsg = "Present Device DD3DDI Removed" + Environment.NewLine + "" + Environment.NewLine + "[ " + r.ToString() + " ]";  //D3DDDIERR_DEVICEREMOVED
                        //if (r == 0x087A0001) sMsg = "Present Device Occluded" + Environment.NewLine + "" + Environment.NewLine + "[ " + r.ToString() + " ]";      //DXGI_STATUS_OCCLUDED
                        //(ignored in the preceding if statement) if (r == 0x887A000A) sMsg = "Present Device Still Drawping" + Environment.NewLine + "" + Environment.NewLine + "[ " + r.ToString() + " ]"; //DXGI_ERROR_WAS_STILL_DRAWING

                        if (sMsg != "") throw (new Exception(sMsg));
                    }

                    calcFps();
                }
            }
            catch (Exception e)
            {
                ShutdownDX2D();
                MessageBox.Show("Problem in DirectX Renderer !" + System.Environment.NewLine + System.Environment.NewLine + "[" + e.ToString() + "]", "DirectX", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return 0;
            }

            return nRet;
        }
        private static string m_sDebugText = "";
        public static string DebugText
        {
            get { return m_sDebugText; }
            set { m_sDebugText = value; }
        }
        private static void calcFps()
        {
            //double t = m_dElapsedFrameStart;// m_objFrameStartTimer.ElapsedMsec;
            m_nFrameCount++;
            if (m_dElapsedFrameStart >= m_fLastTime + 1000)
            {
                double late = m_dElapsedFrameStart - (m_fLastTime + 1000);
                if (late > 2000 || late < 0) late = 0; // ignore if too late
                m_nFps = m_nFrameCount;
                m_nFrameCount = 0;
                m_fLastTime = m_dElapsedFrameStart - late;
            }
        }
        public static int CurrentFPS
        {
            get { return m_nFps; }
            set { 
                m_nFps = value;
            }
        }

        private static bool m_bFrameRateIssue = false;
        public static bool FrameRateIssue
        {
            get { return m_bFrameRateIssue; }
            set { m_bFrameRateIssue = value; }
        }
        private static bool m_bShowFPS = false;
        public static bool ShowFPS
        {
            get { return m_bShowFPS; }
            set { m_bShowFPS = value; }
        }
        private static float m_fLineWidth = 1;
        public static float LineWidth
        {
            get { return m_fLineWidth; }
            set { m_fLineWidth = value; }
        }
        public static bool AntiAlias
        {
            get { return _bAntiAlias; }
            set { 
                _bAntiAlias = value;
                setupAliasing();
            }
        }
        public static bool IsDX2DSetup
        {
            get { return _bDX2Setup; }
        }
        private static bool m_bShowFrameRateIssue = false;
        public static bool ShowFrameRateIssue
        {
            get { return m_bShowFrameRateIssue; }
            set { m_bShowFrameRateIssue = value; }
        }
        private static int m_bLinesPerFrame = 2500;
        public static int LinesPerFrame
        {
            get { return m_bLinesPerFrame; }
            set { m_bLinesPerFrame = value; }
        }
        private static int drawLines()
        {
            Random rnd = new Random();
            SharpDX.Vector2 s = new SharpDX.Vector2(0, 0);
            SharpDX.Vector2 e = new SharpDX.Vector2(0, 0);
            int maxWidth = Target.Width;
            int maxHeight = Target.Height;

            for (int n = 0; n < m_bLinesPerFrame; n++)
            {
                s.X = rnd.Next(maxWidth);
                e.X = rnd.Next(maxWidth);
                s.Y = rnd.Next(maxHeight);
                e.Y = rnd.Next(maxHeight);

                _d2dRenderTarget.DrawLine(s, e, m_DX2lineBrush, m_fLineWidth);
            }

            return m_bLinesPerFrame;
        }
    }
}
