namespace pizzeria
{
     public class Pizzaiolo  // you can alter the signature and contents of this class as needed
                            // it is not allowed to remove code.
    {
        public int _id { get; private set; }
        
        public Pizzaiolo(int id)
        {
            this._id = id;
        }
        
        public void Start()
        {
            life();
        }
        
        public void life() // pizzaiolo: feel free to add instructions to make it thread safe.
        {
            // wait for a customer to order a pizza slice
            Program.order_ready.WaitOne();
            Thread.Sleep(new Random().Next(50, 200));

            PizzaOrder p;
            
            Console.WriteLine($"Pizzaiolo {_id} is about to take the pizza order");
            lock (Program.orderLock)
            {
                p = Program.order.First();
                Program.order.RemoveFirst();
            }
            
            //work on pizza
            Thread.Sleep(new Random().Next(50, 200));
            p.StartWorking();
            Thread.Sleep(new Random().Next(50, 200));
            //finish pizza slice
            Console.WriteLine($"Pizzaiolo {_id} about to finish the pizza slice");
            PizzaSlice s = p.FinishWorking(_id.ToString());


            lock (Program.workingSurfaceLock)
            {
                Program.workingsurface.AddFirst(s);
                // Console.WriteLine($"Pizzaiolo {_id} added slice. Surface count: {Program.workingsurface.Count}");

                if (Program.workingsurface.Count == Program.n_slices)
                {
                    lock (Program.pickUpLock)
                    {
                        Program.pickUp.AddFirst(new PizzaDish(Program.n_slices, $"pizza_{_id}"));
                        // Console.WriteLine($"Pizzaiolo {_id} completed pizza. Pizzas in pickup: {Program.pickUp.Count}");
                    }

                    for (int i = 0; i < Program.n_slices; i++)
                    {
                        Program.slice_ready.Release();
                        // Console.WriteLine($"Pizzaiolo {_id} released slice {i + 1} semaphore");
                    }
                    Program.workingsurface.Clear();
                    // Console.WriteLine($"Pizzaiolo {_id} cleared working surface");
                }
            }
            Console.WriteLine($"Pizzaiolo {_id} finished and terminated");
        }
    }
    
}

