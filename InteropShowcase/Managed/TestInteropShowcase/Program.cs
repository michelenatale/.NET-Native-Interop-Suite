
 

namespace michele.natale; 

public class Program
{
  public static void Main()
  {
    RingBufferTest.Start();
    ReversePInvokeTest.Start();
    VTableTest.Start();

    Console.WriteLine();
    Console.WriteLine("FINISH");
    Console.ReadLine();
  }

  
}