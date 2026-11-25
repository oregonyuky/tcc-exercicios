public class NoFila{
    private No no;
    private NoFila prox;
    private NoFila ant;
    public Fila(){
        this.no = null;
        this.prox = null;
        this.ant = null;
    }
    public NoFila getProx(){
        return this.prox;
    }
    public NoFila getAnt(){
        return this.ant;
    }
    public getNo(){
        return no;
    }
    public setNo(No no){
        return this.no = no;
    }
}