public static unsafe void espelharHorizontal(Bitmap imgS, Bitmap imgD) {
    int width = imgS.Width;
    int height = imgS.Height;
    BitmapData bmS = imgS.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
    BitmapData bmD = imgD.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
    byte* src = (byte*)bmS.Scan0.ToPointer();
    byte* dst = (byte*)bmD.Scan0.ToPointer();
    int strideS = bmS.Stride;
    int strideD = bmD.Stride;
    for (int i = 0; i < height; i++) {
        for (int j = 0; j < width; j++) {
            byte* auxS = src + (strideS * i) + (j * 3);
            byte* auxD = dst + (strideD * i) + ((width - j - 1) * 3);
            auxD[0] = auxS[0];
            auxD[1] = auxS[1];
            auxD[2] = auxS[2];
        }
    }
    imgS.UnlockBits(bmS);
    imgD.UnlockBits(bmD);
}

public static unsafe void espelharVertical(Bitmap imgS, Bitmap imgD) {
    int width = imgS.Width;
    int height = imgS.Height;
    BitmapData bmS = imgS.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
    BitmapData bmD = imgD.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
    byte* src = (byte*)bmS.Scan0.ToPointer();
    byte* dst = (byte*)bmD.Scan0.ToPointer();
    int strideS = bmS.Stride;
    int strideD = bmD.Stride;
    for (int i = 0; i < height; i++) {
        for (int j = 0; j < width; j++) {
            byte* auxS = src + (strideS * i) + (j * 3);
            byte* auxD = dst + (strideD * (height-i-1)) + (j * 3);
            auxD[0] = auxS[0];
            auxD[1] = auxS[1];
            auxD[2] = auxS[2];
        }
    }
    imgS.UnlockBits(bmS);
    imgD.UnlockBits(bmD);
}

public static unsafe void separarPorCor(Bitmap imgS, Bitmap imgD, char cor){
    int width = imgS.Width;
    int height = imgS.Height;
    BitmapData bmS = imgS.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
    BitmapData bmD = imgD.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
    byte* src = (byte*)bmS.Scan0.ToPointer();
    byte* dst = (byte*)bmD.Scan0.ToPointer();
    int sS = bmS.Stride;
    int sD = bmD.Stride;
    for(int i=0;i<height;i++){
        for(int j=0;j<width;j++){
            byte* auxS = src + (sS*i) + (j*3);
            byte* auxD = dst + (sD*i) + (j*3);
            byte r = auxS[2], g = auxS[1], b = auxS[0];
            switch(char.ToUpper(cor)){
                case 'R': g = 0; b = 0; break;
                case 'G': r = 0; b = 0; break;
                case 'B': r = 0; g = 0; break;
            }
            auxD[0] = b;
            auxD[1] = g;
            auxD[2] = r;
        }
    }
    imgS.UnlockBits(bmS);
    imgD.UnlockBits(bmD);
}

public static unsafe void tornarPretoOuBranco(Bitmap imgS, Bitmap imgD, int threshold){
    int width = imgS.Width;
    int height = imgS.Height;
    BitmapData bmS = imgS.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
    BitmapData bmD = imgD.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
    byte* src = (byte*)bmS.Scan0.ToPointer();
    byte* dst = (byte*)bmD.Scan0.ToPointer();
    int sS = bmS.Stride;
    int sD = bmD.Stride;
    for(int i=0;i<height;i++){
        for(int j=0;j<width;j++){
            byte* auxS = src + (sS*i) + (j*3);
            byte* auxD = dst + (sD*i) + (j*3);
            int intens = (auxS[0] + auxS[1] + auxS[2])/3;
            byte valor = (byte)(intens >= threshold ? 255 : 0);
            auxD[0] = valor;
            auxD[1] = valor;
            auxD[2] = valor;
        }
    }
    imgS.UnlockBits(bmS);
    imgD.UnlockBits(bmD);
}

public static unsafe void rotacao90(Bitmap imgS, Bitmap imgD){
    int width = imgS.Width;
    int height = imgS.Height;
    BitmapData bmS = imgS.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
    BitmapData bmD = imgD.LockBits(new Rectangle(0, 0, height, width), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
    byte* src = (byte*)bmS.Scan0.ToPointer();
    byte* dst = (byte*)bmD.Scan0.ToPointer();
    int sS = bmS.Stride;
    int sD = bmD.Stride;
    for(int i=0;i<height;i++){
        for(int j=0;j<width;j++){
            byte* auxS = src + (sS*i) + (j*3);
            byte* auxD = dst + (sD*j) + ((height-i-1)*3);
            auxD[0] = auxS[0];
            auxD[1] = auxS[1];
            auxD[2] = auxS[2];
        }
    }
    imgS.UnlockBits(bmS);
    imgD.UnlockBits(bmD);
}

public static unsafe void inverterRcomB(Bitmap imgS, Bitmap imgD){
    int width = imgS.Width;
    int height = imgS.Height;
    BitmapData bmS = imgS.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
    BitmapData bmD = imgD.LockBits(new Rectangle(0, 0, height, width), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
    byte* src = (byte*)bmS.Scan0.ToPointer();
    byte* dst = (byte*)bmD.Scan0.ToPointer();
    int sS = bmS.Stride;
    int sD = bmD.Stride;
    for(int i=0;i<height;i++){
        for(int j=0;j<width;j++){
            byte* auxS = src + (sS*i) + (j*3);
            byte* auxD = dst + (sD*i) + (j*3);
            byte b = auxS[2];
            byte g = auxS[1];
            byte r = auxS[0];
            auxD[0] = b;
            auxD[1] = g;
            auxD[2] = r;
        }
    }
    imgS.UnlockBits(bmS);
    imgD.UnlockBits(bmD);
}