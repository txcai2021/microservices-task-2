using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieBooking.Models;

namespace MovieBooking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovieBookingController : ControllerBase
    {
        private readonly MovieBookingContext _context;

        public MovieBookingController(MovieBookingContext context)
        {
            _context = context;
        }

        // GET: api/MovieBooking
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MovieBookingTicket>>> GetBookingTickets()
        {
            return await _context.BookingTickets.ToListAsync();
        }

        // GET: api/MovieBooking/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MovieBookingTicket>> GetMovieBookingTicket(int id)
        {
            var movieBookingTicket = await _context.BookingTickets.FindAsync(id);

            if (movieBookingTicket == null)
            {
                return NotFound();
            }

            return movieBookingTicket;
        }

        // PUT: api/MovieBooking/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMovieBookingTicket(int id, MovieBookingTicket movieBookingTicket)
        {
            if (id != movieBookingTicket.Id)
            {
                return BadRequest();
            }

            _context.Entry(movieBookingTicket).State = EntityState.Modified;

            try
            {
                if (movieBookingTicket.Currency != Currencies.SGD)
                    movieBookingTicket.AmountinSGD = GetCurrencyConversionRate(movieBookingTicket.Currency) * movieBookingTicket.Amount;

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MovieBookingTicketExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/MovieBooking
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<MovieBookingTicket>> PostMovieBookingTicket(MovieBookingTicket movieBookingTicket)
        {
            if (movieBookingTicket.Currency != Currencies.SGD)
                movieBookingTicket.AmountinSGD = GetCurrencyConversionRate(movieBookingTicket.Currency) * movieBookingTicket.Amount;
            _context.BookingTickets.Add(movieBookingTicket);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetMovieBookingTicket", new { id = movieBookingTicket.Id }, movieBookingTicket);
        }

        // DELETE: api/MovieBooking/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMovieBookingTicket(int id)
        {
            var movieBookingTicket = await _context.BookingTickets.FindAsync(id);
            if (movieBookingTicket == null)
            {
                return NotFound();
            }

            _context.BookingTickets.Remove(movieBookingTicket);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MovieBookingTicketExists(int id)
        {
            return _context.BookingTickets.Any(e => e.Id == id);
        }


        private decimal GetCurrencyConversionRate(Currencies sourceCurrecy)
        {
            using (var client = new System.Net.Http.HttpClient())
            {
                string uri = "";
                
                switch(sourceCurrecy)
                {
                    case Currencies.USD:
                        uri = "https://cdn.jsdelivr.net/gh/fawazahmed0/currency-api@1/latest/currencies/aud/sgd.json";
                        break;
                    case Currencies.AUD:
                        uri = "https://cdn.jsdelivr.net/gh/fawazahmed0/currency-api@1/latest/currencies/usd/sgd.json";
                        break;

                }

                client.BaseAddress = new Uri(uri);

                try
                {
                    var responseTask = client.GetAsync("");
                    responseTask.Wait();

                    var result = responseTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadAsStringAsync();
                        readTask.Wait();

                        var alldata = readTask.Result;
                        var rate = Newtonsoft.Json.JsonConvert.DeserializeObject<List<CurrencyConversion>>(alldata);
                        return rate.First().sgd;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("{0} Exception caught.", e);
                }

            }
            return 1;
        }
    }
}
