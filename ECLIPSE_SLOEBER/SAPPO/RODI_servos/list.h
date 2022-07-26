// list.h

#ifndef _LIST_h
#define _LIST_h

#if defined(ARDUINO) && ARDUINO >= 100
#include "arduino.h"
#include "string.h"
#else
	#include "WProgram.h"
#endif

#include "node_cpp.h"

template <class T>

class List
{
public:
	List();
	~List();

	void add_head(T);
	void add_head_windows(int num_elementos, T);
	void add_end(T);
	void add_sort(T);
	void concat(List);
	void del_all();
	void del_by_data(T);
	void del_by_position(int);
	void del_head();
	void del_end();
	void print();
	void search(T);
	T *search(int pos);			//Recupera el elemento de la posici�n pos
	T *search_code(long codigo);		//Recupera el elemento con el c�digo codigo
	T *search_head();			//Recupera el primer elemento
	T *search_end();			//Recupera el �ltimo elemento
	void sort();

	int m_num_nodes;

private:
	Node<T> *m_head;
};

#endif

