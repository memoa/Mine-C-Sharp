/*
  Mine
  Program je uradjen prema igrici minesweeper
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mine {
  class Program {

    // Konstante koje koristi klasa MinskoPolje
    const int
      MAX_REDOVA = 20,
      MAX_KOLONA = 35;

    // Status igre objekta klase MinskoPolje
    enum StatusIgre {
      nijeReseno = 0,
      pobeda = 1,
      poraz = 2
    }

    // Povratne vrednosti metode MinskoPolje.OtvorenoPolje()
    enum StatusOtvorenoPolje {
      greskaPogresanUnos = -3,
      greskaResenaPartija = -2,
      greskaVecOtvoreno = -1,
      uRedu = 0
    }

    // Povratne vrednosti metode MinskoPolje.PrikaziPolje()
    enum Polje {
      greska = -1,
      /* 0..8 = otvoreno polje */
      mina = 9,
      nijeOtvoreno = 10
    };

    class MinskoPolje {

      // Svi clanovi klase su privatni
      int brojRedova, brojKolona, brojMina;
      int brojOtvorenihPolja;
      StatusIgre status;
      int[,] matricaMina;
      bool[,] otvorenoPolje;

      // Getteri za pojedine clanove klase
      public int BrojRedova { get { return brojRedova; } }
      public int BrojKolona { get { return brojKolona; } }
      public StatusIgre Status { get { return status; } }

      // Konstruktor
      public MinskoPolje(int iBrojRedova = 10, int iBrojKolona = 10, int iBrojMina = 8) {
        if (

          // Zastita od pogresnog ulaza, default vrednosti
          iBrojRedova < 1 || iBrojRedova > MAX_REDOVA ||
          iBrojKolona < 1 || iBrojKolona > MAX_KOLONA ||
          iBrojMina < 0 || iBrojMina > iBrojRedova * iBrojKolona) {
          brojRedova = 10;
          brojKolona = 10;
          brojMina = 8;
        }
        else {

          // Ako su ulazne vredvosti u redu, proslediti ih clanovima objekta
          brojRedova = iBrojRedova;
          brojKolona = iBrojKolona;
          brojMina = iBrojMina;
        }

        /* Debug
          Console.WriteLine("Broj redova: "+brojRedova);
          Console.WriteLine("Broj kolona: "+brojKolona);
          Console.WriteLine("Broj mina: "+brojMina);
        */

        // Inicijalizacija preostalih clanova objekta
        brojOtvorenihPolja = 0;
        status = StatusIgre.nijeReseno;
        matricaMina = new int[brojRedova, brojKolona];
        otvorenoPolje = new bool[brojRedova, brojKolona];

        // Generisanje slucajnih brojeva, postavljanje mina
        Random random = new Random();
        for (int i = 0; i < brojMina; ++i) {
          int mina = random.Next(0, brojRedova * brojKolona);

          /* Debug
            Console.WriteLine("Random: "+mina);
            Console.WriteLine("red: "+mina/brojKolona);
            Console.WriteLine("kol: "+mina%brojKolona);
          */

          // U slucaju da se ponovi ista mina, prebaciti je na sledece prazno mesto
          // Ovim nacinom je skraceno vreme postavljanja mina
          while (matricaMina[mina / brojKolona, mina % brojKolona] == 9) {
            mina++;
            if (mina == brojRedova * brojKolona)
              mina = 0;
          }
          matricaMina[mina / brojKolona, mina % brojKolona] = 9;
        }
      }

      // Metoda za pristup poljima objekta u svrhu prikaza na ekran
      public Polje PrikaziPolje(int red, int kolona) {
        // Zastita od pogresnog ulaza podataka
        if (red < 0 || red >= brojRedova || kolona < 0 || kolona >= brojKolona)
          return Polje.greska;
        // Prikazi izlaznu vrednost za zadato polje
        else if (otvorenoPolje[red, kolona])
          return (Polje)matricaMina[red, kolona];
        else
          return Polje.nijeOtvoreno;
      }

      // Otvaranje polja, prebrojavanje mina oko zadatog polja
      // i otvaranje polja oko zadatog polja ako nema mina oko zadatog polja
      public StatusOtvorenoPolje OtvoriPolje(int red, int kolona) {

        // Zastita od pogresnog ulaza podataka
        if (red < 0 || red >= brojRedova || kolona < 0 || kolona >= brojKolona)
          return StatusOtvorenoPolje.greskaPogresanUnos;

        // Zastita od otvaranja vec otvorenih polja i otvaranja polja ako je igra zavrsena
        // Izlazni uslov u slucaju rekurzivnih poziva
        if (status == StatusIgre.nijeReseno && !otvorenoPolje[red, kolona]) {
          int nadjenoMina = 0;
          otvorenoPolje[red, kolona] = true;

          // Otvorena mina, otvoriti ostale mine i zavrsiti
          if (matricaMina[red, kolona] == 9) {
            for (int i = 0; i < brojRedova; i++)
              for (int j = 0; j < brojKolona; j++)
                if (matricaMina[i, j] == 9)
                  otvorenoPolje[i, j] = true;
            status = StatusIgre.poraz;
            return StatusOtvorenoPolje.uRedu;
          }

          // Brojanje mina oko otvorenog polja

          // Gore levo
          if (red - 1 >= 0 && kolona - 1 >= 0)
            if (matricaMina[red - 1, kolona - 1] == 9)
              nadjenoMina++;

          // Gore
          if (red - 1 >= 0)
            if (matricaMina[red - 1, kolona] == 9)
              nadjenoMina++;

          // Gore desno
          if (red - 1 >= 0 && kolona + 1 < brojKolona)
            if (matricaMina[red - 1, kolona + 1] == 9)
              nadjenoMina++;

          // Desno
          if (kolona + 1 < brojKolona)
            if (matricaMina[red, kolona + 1] == 9)
              nadjenoMina++;

          // Dole desno
          if (red + 1 < brojRedova && kolona + 1 < brojKolona)
            if (matricaMina[red + 1, kolona + 1] == 9)
              nadjenoMina++;

          // Dole
          if (red + 1 < brojRedova)
            if (matricaMina[red + 1, kolona] == 9)
              nadjenoMina++;

          // Dole levo
          if (red + 1 < brojRedova && kolona - 1 >= 0)
            if (matricaMina[red + 1, kolona - 1] == 9)
              nadjenoMina++;

          // Levo
          if (kolona - 1 >= 0)
            if (matricaMina[red, kolona - 1] == 9)
              nadjenoMina++;

          // Mine su prebrojane, upis broja u matricu za prikaz
          matricaMina[red, kolona] = nadjenoMina;
          brojOtvorenihPolja++;

          // Ako su otvorena sva polja, kraj
          if (brojOtvorenihPolja == brojRedova * brojKolona - brojMina) {
            for (int i = 0; i < brojRedova; i++)
              for (int j = 0; j < brojKolona; j++)
                if (!otvorenoPolje[i, j])
                  otvorenoPolje[i, j] = true;
            status = StatusIgre.pobeda;
            return StatusOtvorenoPolje.uRedu;
          }

          /* debug
            Console.WriteLine("Otvoreno polje: [{0}, {1}]", red, kolona);
            Console.WriteLine("brojOtvorenihPolja: "+brojOtvorenihPolja);
          */

          // Nema mina okolo, otvoriti sva polja okolo (rekurzivni pozivi)
          if (nadjenoMina == 0) {

            // Gore levo
            if (red - 1 >= 0 && kolona - 1 >= 0)
              OtvoriPolje(red - 1, kolona - 1);

            // Gore
            if (red - 1 >= 0)
              OtvoriPolje(red - 1, kolona);

            // Gore desno
            if (red - 1 >= 0 && kolona + 1 < brojKolona)
              OtvoriPolje(red - 1, kolona + 1);

            // Desno
            if (kolona + 1 < brojKolona)
              OtvoriPolje(red, kolona + 1);

            // Dole desno
            if (red + 1 < brojRedova && kolona + 1 < brojKolona)
              OtvoriPolje(red + 1, kolona + 1);

            // Dole
            if (red + 1 < brojRedova)
              OtvoriPolje(red + 1, kolona);

            // Dole levo
            if (red + 1 < brojRedova && kolona - 1 >= 0)
              OtvoriPolje(red + 1, kolona - 1);

            // Levo
            if (kolona - 1 >= 0)
              OtvoriPolje(red, kolona - 1);
          }
          return StatusOtvorenoPolje.uRedu; // Sve je u redu
        }
        else { // Polje je vec otvoreno ili je zavrsena igra
          if (otvorenoPolje[red, kolona])
            return StatusOtvorenoPolje.greskaVecOtvoreno;
          else
            return StatusOtvorenoPolje.greskaResenaPartija;
        }
      }
    }

    // Izvrsavanje programa
    static void Main(string[] args) {

      // Korisnicki interfejs
      int brojRedova, brojKolona, brojMina, red, kolona;
      Console.WriteLine("*** Mine ***");

      // Ponavljanje novih partija igre sve dok korisnik to zeli
      while (true) {

        // Unos podataka za kreiranje igre
        // i zastita od unosa nedozvoljenih vrednosti
        while (true) {
          try {
            Console.WriteLine("Kreiranje igre");
            do {
              Console.Write("Unesite broj redova [1-{0}]: ", MAX_REDOVA);
              brojRedova = Convert.ToInt16(Console.ReadLine());
            } while (brojRedova < 1 || brojRedova > MAX_REDOVA);
            do {
              Console.Write("Unesite broj kolona[1-{0}]: ", MAX_KOLONA);
              brojKolona = Convert.ToInt16(Console.ReadLine());
            } while (brojKolona < 1 || brojKolona > MAX_KOLONA);
            do {
              Console.Write("Unesite broj mina[0-{0}]: ", brojRedova * brojKolona);
              brojMina = Convert.ToInt16(Console.ReadLine());
            } while (brojMina < 0 || brojMina > brojRedova * brojKolona);
            break;
          }
          catch (Exception e) {
            Console.WriteLine("Greska pri unosu! Pokusajte ponovo.");
            continue;
          }
        }

        /* Debug
          Console.WriteLine("Broj redova: "+brojRedova);
          Console.WriteLine("Broj kolona: "+brojKolona);
          Console.WriteLine("Broj mina: "+brojMina);
        */

        // Kreiranje igre
        MinskoPolje matricaMina = new MinskoPolje(brojRedova, brojKolona, brojMina);

        // Unosenje koordinata za otvaranje polja objekta matricaMina
        // i zastita od unosa nedozvoljenih vrednosti
        while (true) {

          // Ako partija nije zavrsena, prikaz minskog polja
          if (matricaMina.Status == StatusIgre.nijeReseno) {

            // Unos koordinata za otvaranje polja
            // i zastita od unosa nedozvoljenih vrednosti
            while (true) {
              try {
                Console.WriteLine("Otvaranje Polja");
                do {
                  Console.Write("Unesite red[1-{0}]: ", matricaMina.BrojRedova);
                  red = Convert.ToInt16(Console.ReadLine()) - 1;
                } while (red < 0 || red >= matricaMina.BrojRedova);
                do {
                  Console.Write("Unesite kolonu[1-{0}]: ", matricaMina.BrojKolona);
                  kolona = Convert.ToInt16(Console.ReadLine()) - 1;
                } while (kolona < 0 || kolona >= matricaMina.BrojKolona);
                break;
              }
              catch (Exception e) {
                Console.WriteLine("Greska pri unosu! Pokusajte ponovo.");
                continue;
              }
            }

            // Otvaranje polja
            StatusOtvorenoPolje statusOtvorenoPolje = matricaMina.OtvoriPolje(red, kolona);

            // Prikaz minskog polja
            for (int i = 0; i < matricaMina.BrojRedova; i++) {
              for (int j = 0; j < matricaMina.BrojKolona; j++) {
                Polje polje = matricaMina.PrikaziPolje(i, j);

                // Prikaz otvorenog polja gde nije postavljena mina
                if ((int)polje >= 0 && (int)polje <= 8) {
                  Console.ForegroundColor = (int)polje == 8 ? ConsoleColor.White : (ConsoleColor)((int)polje + 8);
                  Console.Write((int)polje + " ");
                  Console.ForegroundColor = ConsoleColor.Gray;

                  // Prikaz otvorenog polja gde je postavljena mina
                }
                else if (polje == Polje.mina) {
                  if (matricaMina.Status == StatusIgre.pobeda)
                    Console.ForegroundColor = ConsoleColor.Yellow;
                  else if (matricaMina.Status == StatusIgre.poraz)
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                  Console.Write("* ");
                  Console.ForegroundColor = ConsoleColor.Gray;

                  // Prikaz polja koje nije otvoreno
                }
                else if (polje == Polje.nijeOtvoreno) {
                  Console.Write("? ");
                }
              }

              // Novi red nakon prikaza kompletnog minskog polja
              Console.WriteLine();
            }

            // Ukoliko se otvara vec otvoreno polje, izbaciti gresku
            // i poslati na ponovni unos koordinata za otvaranje polja
            if (statusOtvorenoPolje == StatusOtvorenoPolje.greskaVecOtvoreno) {
              Console.WriteLine("Greska. Polje sa koordinatama ({0}, {1}) je vec otvoreno!", red + 1, kolona + 1);
              continue;
            }

            Console.WriteLine("Izlaz <Ctrl+C>");

          }
          else { // Ako je partija zavrsena, ispis poruke na ekran
            if (matricaMina.Status == StatusIgre.pobeda)
              Console.WriteLine("Cestitamo, pobedili ste.");
            else
              Console.WriteLine("Nazalost, izgubili ste.");

            // Mogucnost izlaza iz igre ili odabir nove partije
            Console.Write("Zelite li nastaviti [d/n]: ");
            ConsoleKeyInfo odgovor = Console.ReadKey();
            Console.WriteLine();
            if (odgovor.KeyChar == 'n' || odgovor.KeyChar == 'N')
              return; // Izlaz iz programa
            break; // Kreiranje nove igre
          }
        }
      }
    }
  }
}