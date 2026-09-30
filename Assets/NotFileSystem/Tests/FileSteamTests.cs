using NotBura.Packages;
using NUnit.Framework;
using System;
using System.IO;
using Unity.Collections.LowLevel.Unsafe;
using Unity.PerformanceTesting;
using Unity.PerformanceTesting.Measurements;
using UnityEditor;

public sealed class FileSteamTests
{
    private string m_path;
    private string m_text;

    [OneTimeSetUp]
    public void SetUp()
    {
        m_path= Path.GetFullPath("Assets/test.txt");
        m_text = "寿限無寿限無後光の擦り切れ";
    }

    [OneTimeTearDown]
    public void TearDown()
    {
        AssetDatabase.Refresh();
    }

    [Test]
    [Performance]
    public void SpeedWriteFileStreamTest()
    {
        var _measurement = Measure.Method(WriteFileStreamInternal);
        Run(_measurement);
    }

    [Test]
    [Performance]
    public void SpeedWriteFileHandleTest()
    {
        var _measurement = Measure.Method(WriteFileHandleInternal);
        Run(_measurement);
    }

    [Test]
    [Performance]
    public void SpeedReadeFileStreamTest()
    {
        var _measurement = Measure.Method(ReadFileStreamInternal);
        Run(_measurement);
    }

    [Test]
    [Performance]
    public void SpeedReadFileHandleTest()
    {
        var _measurement = Measure.Method(ReadFileHandleInternal);
        Run(_measurement);
    }

    [Test]
    public unsafe void WriteFileStreamInternal()
    {
        using var _stream = File.OpenWrite(m_path);
        fixed (char* _pointer = m_text)
        {
            var _span = new ReadOnlySpan<byte>(_pointer, m_text.Length << 1);
            _stream.Write(_span);
        }
    }

    [Test]
    public unsafe void WriteFileHandleInternal()
    {
        using var _handle = NotFileSystem.OpenWrite(m_path);
        _handle.Write(m_text);
    }

    [Test]
    public unsafe void ReadFileStreamInternal()
    {
        using var _stream = File.OpenRead(m_path);

        var _result = new byte[_stream.Length];
        _stream.Read(_result);
    }

    [Test]
    public unsafe void ReadFileHandleInternal()
    {
        using var _handle = NotFileSystem.OpenRead(m_path);

        var _result = new byte[_handle.GetSize()];
        _handle.Read(_result);
    }

    private static unsafe void Log(byte[] source)
    {
        var _buffer = new string('\0', source.Length);
        fixed (byte* _source = source)
        fixed (char* _destination = _buffer)
        {
            UnsafeUtility.MemCpy(_destination, _source, source.Length);
            UnityEngine.Debug.Log(_buffer);
        }
    }

    private static void Run(MethodMeasurement measurement)
    {
        measurement
            .WarmupCount(10)
            .IterationsPerMeasurement(50)
            .MeasurementCount(50)
            .GC()
            .Run();
    }
}
