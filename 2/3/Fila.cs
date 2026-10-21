public class Fila{
    public NoFila inicio;
    public NoFila fim;
    public Fila(){
        this.inicio = null;
        this.fim = null;
    }
    public void enqueue(No no){
        NoFila aux = new NoFila();
        aux.setNo(no);
        if(inicio==null){
            inicio = aux;
            fim = aux;
        } else {
            fim.setProx(aux);
            fim = aux;
        }
    }
    public No dequeue(){
        NoFila aux = inicio;
        inicio = inicio.getProx();
        if(inicio==null)fim=null;
        return aux;
    }
    public bool isEmpty(){
        return inicio == null;
    }

}