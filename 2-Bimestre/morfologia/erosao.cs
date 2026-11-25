using System.Drawing;
using System.Drawing.Imaging;

public class Erosao
{
    BitmapData bmS;
    BitmapData bmA;
    BitmapData bmD;

    public unsafe byte* getP(int i, int j){
        return (byte*)bmS.Scan0 + bmS.Stride * i + j*3;
    }

    public unsafe byte* getPA(int i, int j){
        return (byte*)bmA.Scan0 + bmA.Stride * i + j*3;
    }

    public unsafe byte* getPD(int i, int j){
        return (byte*)bmD.Scan0 + bmD.Stride * i + j*3;
    }

    public bool ehPreto(int p){
        return p == 0;
    }

    public unsafe void erosao(Bitmap imgS, Bitmap imgA, Bitmap imgD){

        int width = imgS.Width;
        int height = imgS.Height;
        int widthA = imgA.Width;
        int heightA = imgA.Height;

        // Origem do elemento estruturante
        int x = 0;
        int y = 1;

        Rectangle r = new Rectangle(0,0,width,height);
        Rectangle rA = new Rectangle(0,0,widthA,heightA);

        bmS = imgS.LockBits(r, ImageLockMode.ReadOnly,
                            PixelFormat.Format24bppRgb);

        bmA = imgA.LockBits(rA, ImageLockMode.ReadOnly,
                            PixelFormat.Format24bppRgb);

        bmD = imgD.LockBits(r, ImageLockMode.ReadWrite,
                            PixelFormat.Format24bppRgb);

        // Inicializar a imagem destino com branco
        for(int i=0;i<height;i++){
            for(int j=0;j<width;j++){
                byte* aux = getPD(i,j);
                aux[0] = aux[1] = aux[2] = 255;
            }
        }

        // Erosão
        for(int i=-y;i<height-y;i++){
            for(int j=-x;j<width-x;j++){

                bool flag = false;

                byte* auxD = getPD(i+y, j+x);

                for(int k=0;k<heightA;k++){
                    for(int l=0;l<widthA;l++){

                        int ny = i+k;
                        int nx = j+l;

                        byte* auxA = getPA(k,l);

                        if(ehPreto(auxA[0])){

                            if(ny>=0 && ny<height &&
                               nx>=0 && nx<width){

                                byte* aux = getP(ny,nx);

                                if(!ehPreto(aux[0])){
                                    flag = true;
                                }

                            }else{
                                flag = true;
                            }
                        }
                    }
                }

                if(flag){
                    auxD[0] = auxD[1] = auxD[2] = 255;
                }else{
                    auxD[0] = auxD[1] = auxD[2] = 0;
                }
            }
        }

        imgS.UnlockBits(bmS);
        imgA.UnlockBits(bmA);
        imgD.UnlockBits(bmD);
    }
}