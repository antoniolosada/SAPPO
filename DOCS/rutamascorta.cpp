#define MAX 10005 //maximo numero de vértices
#define Node pair< int , int > //definimos el nodo como un par( first , second ) donde first es el vertice adyacente y second el peso de la arista
#define INF 1<<30 //definimos un valor grande que represente la distancia infinita inicial, basta conque sea superior al maximo valor del peso en alguna de las aristas

vector< Node > ady[ MAX ]; //lista de adyacencia
int distancia[ MAX ];      //distancia[ u ] distancia de vértice inicial a vértice con ID = u
bool visitado[ MAX ];      //para vértices visitados
int previo[ MAX ];         //para la impresion de caminos
priority_queue< Node , vector<Node> , cmp > Q; //priority queue propia del c++, usamos el comparador definido para que el de menor valor este en el tope
int V;                     //numero de vertices

//función de inicialización
void init(){
    for( int i = 0 ; i <= V ; ++i ){
        distancia[ i ] = INF;  //inicializamos todas las distancias con valor infinito
        visitado[ i ] = false; //inicializamos todos los vértices como no visitado
        previo[ i ] = -1;      //inicializamos el previo del vértice i con -1
    }
}

Q.push( Node( inicial , 0 ) ); //Insertamos el vértice inicial en la Cola de Prioridad
distancia[ inicial ] = 0;      //Este paso es importante, inicializamos la distancia del inicial como 0

while( !Q.empty() ){                   //Mientras cola no este vacia
        actual = Q.top().first;            //Obtengo de la cola el nodo con menor peso, en un comienzo será el inicial
        Q.pop();                           //Sacamos el elemento de la cola
		
		if( visitado[ actual ] ) continue; //Si el vértice actual ya fue visitado entonces sigo sacando elementos de la cola
		
		for( int i = 0 ; i < ady[ actual ].size() ; ++i ){ //reviso sus adyacentes del vertice actual
    adyacente = ady[ actual ][ i ].first;   //id del vertice adyacente
    peso = ady[ actual ][ i ].second;        //peso de la arista que une actual con adyacente ( actual , adyacente )
    if( !visitado[ adyacente ] ){        //si el vertice adyacente no fue visitado
	
	
	for( int i = 0 ; i < ady[ actual ].size() ; ++i ){ //reviso sus adyacentes del vertice actual
    adyacente = ady[ actual ][ i ].first;   //id del vertice adyacente
    peso = ady[ actual ][ i ].second;        //peso de la arista que une actual con adyacente ( actual , adyacente )
    if( !visitado[ adyacente ] ){        //si el vertice adyacente no fue visitado
        relajacion( actual , adyacente , peso ); //realizamos el paso de relajacion
    }
}

//Paso de relajacion
void relajacion( int actual , int adyacente , int peso ){
    //Si la distancia del origen al vertice actual + peso de su arista es menor a la distancia del origen al vertice adyacente
    if( distancia[ actual ] + peso < distancia[ adyacente ] ){
        distancia[ adyacente ] = distancia[ actual ] + peso;  //relajamos el vertice actualizando la distancia
        previo[ adyacente ] = actual;                         //a su vez actualizamos el vertice previo
        Q.push( Node( adyacente , distancia[ adyacente ] ) ); //agregamos adyacente a la cola de prioridad
    }
}

//Impresion del camino mas corto desde el vertice inicial y final ingresados
void print( int destino ){
    if( previo[ destino ] != -1 )    //si aun poseo un vertice previo
        print( previo[ destino ] );  //recursivamente sigo explorando
    printf("%d " , destino );        //terminada la recursion imprimo los vertices recorridos
}

