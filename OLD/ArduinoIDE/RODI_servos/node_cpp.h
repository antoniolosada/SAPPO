// node_cpp.h

#ifndef _NODE_CPP_h
#define _NODE_CPP_h

#if defined(ARDUINO) && ARDUINO >= 100
	#include "arduino.h"
#else
	#include "WProgram.h"
#endif
using namespace std;

template <class T>

class Node
{
public:
	Node();
	Node(T);
	~Node();

	Node *next;
	T data;

	void delete_all();
	void print();
};

#endif

