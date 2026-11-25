public class dilatacao(){
    Bitmap imgS;
    Bitmap imgA;
    Bitmap imgD;
    public unsafe byte* getP(int i, int j){
         return imgS.Scan0 + img.Stride * i + j*3;
    }
    public unsafe byte* getPA(int i, int j){
         return imgA.Scan0 + imgA.Stride * i + j*3;
    }
    public unsafe boolean ehPreto(int p){
        return p==0;
    }
    public unsafe void dilatacao(Bitmap imgS, Bitmap imgA, Bitmap imgD){
        int width = imgS.Width;
        int height = imgS.Height;
        int widthA = imgA.Width;
        int heightA = imgA.Height;
        byte* meio = imgA.Stride*heightA/2 + 3 * widthA/2;
        Rectangle r = new Rectangle(0,0,width,height);
        BitmapData *bmS = imgS.LockBits(r, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
        BitmapData *bmD = imgD.LockBits(r, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
        for(int i=0;i<height;i++){
            for(int j=0;j<width;j++){
                boolean flag = false;
                bool flag = false;

                int ny = i + heightA/2;
                int nx = j + widthA/2;

                if(ny>=0 && ny<height && nx>=0 && nx<width){

                    byte* aux = getP(ny,nx);

                    if(ehPreto(aux[0]) && ehPreto(meio[0])){
                        flag = true;
                    }
                }
                if(flag){
                    for(int k=0;k<heightA;k++){
                        for(int l=0;l<widthA;l++){
                            int ny = i + k;
                            int nx = j + l;

                            if(ny>=0 && ny<height && nx>=0 && nx<width){

                                byte* auxA = getPA(k,l);
                                byte* auxD = getPD(ny,nx);

                                if(ehPreto(auxA[0])){
                                    auxD[0] = auxD[1] = auxD[2] = auxA[0];
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}