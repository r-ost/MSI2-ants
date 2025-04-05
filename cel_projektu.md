Projekt 1
Zastosowanie algorytmu mrówkowego do rozwiązania problemu Capacitated Vehicle Routing Problem. Rozważamy wariant CVRP, gdize:

liczba pojazdów jest ustalona, podana na wejściu,
droga, którą może przebyć każdy z pojazdów jest ograniczona przez wartość 
s
m
a
x
 (taka sama dla każdego pojazdu), podaną na wejśiu.
Należy porównać ze sobą następujące podejścia:

algorytm mrówkowy w podstawowej wersji,
algorytm mrówkowy z modyfikacją zaproponowaną przez siebie lub zaczerpniętą z literatury – zaproponować dwie różne modyfikacje i sprawdzić obie,
algorytm zachłanny.
Do przetestowania algorytmu należy wykorzystać wybrany przez siebie zbiór lub zbiory testowe, które zapewnią:

zróżnicowanie przypadków testowych (w szczególności pod względem rozmiaru i struktury grafu),
rozsądną liczbę przypadków testowych, żeby dało się wyciągnąć ogólne wnioski.
Przy stawianiu hipotez uwzględnić porównanie skuteczności i czasu obliczeń poszczególnych podejść, uwzględniając rozmiar wejściowego problemu i różne warianty parametryzacji algorytmu mrówkowego.

Capacitated Vehicle Routing Problem (CVRP) to problem optymalizacyjny, który jest rozszerzeniem problemu
marszrutyzacji (Vehicle Routing Problem - VRP). W zadaniu VRP mamy dany nieskierowany graf pełny oraz
ustaloną liczbę pojazdów. Wierzchołki grafu, poza jednym, oznaczają lokalizacje klientów na płaszczyźnie.
Jeden wierzchołek jest wyszczególniony i oznacza magazyn. Krawędzie w grafie mają przypisane wagi, które
oznaczają odległości między wierzchołkami. Celem problemu VRP jest odwiedzenie każdego klienta dokładnie
raz, minimalizując całkowity koszt przebycia tras wszystkich pojazdów. Każda trasa zaczyna się i kończy w
magazynie. W CVRP dodatkowo każdy klient ma określone zapotrzebowanie na jeden, określony produkt, a
każdy pojazd może przewozić skończoną ilość produktu. W ramach projektu zakładamy dodatkowo, że droga,
którą może przebyć każdy z pojazdów, jest ograniczona przez pewną stałą.
Problem CVRP jest NP-trudny [1]. W związku z tym dokładne rozwiązanie można wyznaczyć tylko dla małych
instancji (rzędu kilku-kilkunastu wierzchołków). W praktyce problem CVRP rozwiązuje się, znajdując
przybliżone rozwiązanie. W projekcie zostaną porównane cztery algorytmy znajdujące przybliżone rozwiązanie
problemu CVRP:
Algorytm mrówkowy
Algorytm mrówkowy z heurystyką 2-opt
Algorytm mrówkowy z modyfikacją Max-Min (Max-Min ant system - MMAS)
Algorytm zachłanny
