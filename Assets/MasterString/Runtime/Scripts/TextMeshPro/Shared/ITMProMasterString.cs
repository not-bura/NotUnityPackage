using System;

namespace NotBura.Packages
{
    public interface ITMProMasterString
    {
        public void SetText(ReadOnlySpan<char> source);
        public void SetText(MasterString source);
        public void SetText(ValueString source);

        #region SetText for PrimitiveType

        public void SetText(byte source) {}
        public void SetText(sbyte source) {}
        public void SetText(short source) {}
        public void SetText(ushort source) {}
        public void SetText(int source) { }
        public void SetText(uint source) { }
        public void SetText(nint source) { }
        public void SetText(nuint source) { }
        public void SetText(long source) { }
        public void SetText(ulong source) { }
        public void SetText(float source) { }
        public void SetText(double source) { }
        public void SetText(decimal source) { }
        public void SetText(char source) { }
        //public void SetText(bool source);

        #endregion SetText for PrimitiveType
    }
}
