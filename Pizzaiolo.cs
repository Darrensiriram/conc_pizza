namespace pizzeria
{
     public class Pizzaiolo
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
            try
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
                PizzaSlice s = p.FinishWorking(_id.ToString()); //feel free to change the init of the ID
                                                                // with anything that can help you debug
    
                lock (Program.workingSurfaceLock)
                {
                      Program.workingsurface.AddFirst(s);
                      if (Program.workingsurface.Count == Program.n_slices)
                      {
                          lock (Program.pickUpLock)
                          {
                              Program.pickUp.AddFirst( new PizzaDish(Program.n_slices, s.ToString()));
                          }
                          Program.slice_ready.Release(Program.n_slices);
                          Program.workingsurface.Clear();
                      }
                }
                
                Console.WriteLine($"Pizzaiolo{_id} finished and goes to sleep.");
                
            }
            catch (Exception e)
            {
                throw new Exception($"Pizzaiolo {_id} [MISTAKE!]");
            }
            
        }
    }
}

