// 
// 
// 

#include "list.h"

// Constructor por defecto
template<typename T>
List<T>::List()
{
	m_num_nodes = 0;
	m_head = NULL;
}
// Insertar al inicio manteniendo una ventana de elementos m�ximo de num_elementos
template<typename T>
void List<T>::add_head_windows(int num_elementos, T data_)
{
	Node<T> *new_node = new Node<T>(data_);
	Node<T> *temp = m_head;

	if (!m_head) {
		m_head = new_node;
	}
	else {
		new_node->next = m_head;
		m_head = new_node;

		while (temp) {
			temp = temp->next;
		}
	}
	m_num_nodes++;

	if (m_num_nodes > num_elementos)
		del_end();
}

// Insertar al inicio
template<typename T>
void List<T>::add_head(T data_)
{
	Node<T> *new_node = new Node<T>(data_);
	Node<T> *temp = m_head;

	if (!m_head) {
		m_head = new_node;
	}
	else {
		new_node->next = m_head;
		m_head = new_node;

		while (temp) {
			temp = temp->next;
		}
	}
	m_num_nodes++;
}

// Insertar al final
template<typename T>
void List<T>::add_end(T data_)
{
	Node<T> *new_node = new Node<T>(data_);
	Node<T> *temp = m_head;

	if (!m_head) {
		m_head = new_node;
	}
	else {
		while (temp->next != NULL) {
			temp = temp->next;
		}
		temp->next = new_node;
	}
	m_num_nodes++;
}

// Insertar de manera ordenada
template<typename T>
void List<T>::add_sort(T data_)
{
	Node<T> *new_node = new Node<T>(data_);
	Node<T> *temp = m_head;

	if (!m_head) {
		m_head = new_node;
	}
	else {
		if (m_head->data > data_) {
			new_node->next = m_head;
			m_head = new_node;
		}
		else {
			while ((temp->next != NULL) && (temp->next->data < data_)) {
				temp = temp->next;
			}
			new_node->next = temp->next;
			temp->next = new_node;
		}
	}
	m_num_nodes++;
}

// Concatenar a otra List
template<typename T>
void List<T>::concat(List list)
{
	Node<T> *temp2 = list.m_head;

	while (temp2) {
		add_end(temp2->data);
		temp2 = temp2->next;
	}
}

// Eliminar todos los nodos
template<typename T>
void List<T>::del_all()
{
	m_head->delete_all();
	m_head = 0;
}

// Eliminar por data del nodo
template<typename T>
void List<T>::del_by_data(T data_)
{
	Node<T> *temp = m_head;
	Node<T> *temp1 = m_head->next;

	int cont = 0;

	if (m_head->data == data_) {
		m_head = temp->next;
	}
	else {
		while (temp1) {
			if (temp1->data == data_) {
				Node<T> *aux_node = temp1;
				temp->next = temp1->next;
				delete aux_node;
				cont++;
				m_num_nodes--;
			}
			temp = temp->next;
			temp1 = temp1->next;
		}
	}

	if (cont == 0) {
		Serial.println("No existe el dato ");
	}
}

template<typename T>
void List<T>::del_head()
{
	del_by_position(1);
}

template<typename T>
void List<T>::del_end()
{
	del_by_position(m_num_nodes);
}

// Eliminar por posici�n del nodo
template<typename T>
void List<T>::del_by_position(int pos)
{
	Node<T> *temp = m_head;
	Node<T> *temp1 = temp->next;

	if (pos < 1 || pos > m_num_nodes) {
		Serial.println("Fuera de rango");
	}
	else if (pos == 1) {
		m_head = temp->next;
	}
	else {
		for (int i = 2; i <= pos; i++) {
			if (i == pos) {
				Node<T> *aux_node = temp1;
				temp->next = temp1->next;
				delete aux_node;
				m_num_nodes--;
			}
			temp = temp->next;
			temp1 = temp1->next;
		}
	}
}

// Usado por el m�todo intersecci�n
template<typename T>
void insert_sort(T a[], int size)
{
	T temp;
	for (int i = 0; i < size; i++) {
		for (int j = i - 1; j >= 0 && a[j + 1] < a[j]; j--) {
			temp = a[j + 1];
			a[j + 1] = a[j];
			a[j] = temp;
		}
	}
}


// Imprimir la Lista
template<typename T>
void List<T>::print()
{
	Node<T> *temp = m_head;
	if (!m_head) {
		Serial.println("La Lista est� vac�a ");
	}
	else {
		while (temp) {
			temp->data.print();
			if (!temp->next) Serial.println("NULL");
			temp = temp->next;
		}
	}
}

// Buscar el dato de un nodo
template<typename T>
void List<T>::search(T data_)
{
	Node<T> *temp = m_head;
	int cont = 1;
	int cont2 = 0;

	while (temp) {
		if (temp->data == data_) {
			Serial.print("El dato se encuentra en la posici�n: ");
			Serial.println(cont);
			cont2++;
		}
		temp = temp->next;
		cont++;
	}

	if (cont2 == 0) {
		Serial.print("No existe el dato ");
	}
}

//Posici�n desde 0
template<typename T>
T *List<T>::search(int pos)
{
	Node<T> *temp = m_head;
	int i;
	pos--;
	for(i = 0; i < m_num_nodes; i++)
	{
		if (pos == i)
			break;
		else
			temp = temp->next;
	}
	if (i < m_num_nodes)
		return &(temp->data);
	else
		return NULL;
}

template<typename T>
T *List<T>::search_code(long codigo)
{
	Node<T> *temp = m_head;
	int cont = 1;
	int cont2 = 0;

	while (temp) {
		if (temp->data.codigo == codigo) {
			return &(temp->data);
			cont2++;
		}
		temp = temp->next;
		cont++;
	}

	if (cont2 == 0) {
		return NULL;
	}
}

template<typename T>
T *List<T>::search_head()
{
	return m_head;
}

template<typename T>
T *List<T>::search_end()
{
	Node<T> *temp = m_head;

	if (!temp) return NULL; //Lista vac�a
	while (temp->next);
	return temp;
}

// Ordenar de manera ascendente
template<typename T>
void List<T>::sort()
{
	T temp_data;
	Node<T> *aux_node = m_head;
	Node<T> *temp = aux_node;

	while (aux_node) {
		temp = aux_node;

		while (temp->next) {
			temp = temp->next;

			if (aux_node->data > temp->data) {
				temp_data = aux_node->data;
				aux_node->data = temp->data;
				temp->data = temp_data;
			}
		}

		aux_node = aux_node->next;
	}
}

template<typename T>
List<T>::~List() {}
