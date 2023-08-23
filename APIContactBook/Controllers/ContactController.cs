using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using ContactBookApi.Models;
using Microsoft.AspNetCore.Authorization;

namespace ContactBookApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactController : ControllerBase
    {
        private readonly ContactApiContext _dbContext;

        public ContactController(ContactApiContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("all")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> GetAllContacts([FromQuery] int page = 1)
        {
            const int pageSize = 10;
            var contacts = await _dbContext.Contacts
                .OrderBy(c => c.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(contacts);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "ADMIN,REGULAR")]
        public async Task<IActionResult> GetContactById(int id)
        {
            var contact = await _dbContext.Contacts.FindAsync(id);
            if (contact == null)
                return NotFound();

            return Ok(contact);
        }

        [HttpGet("Search")]
        [AllowAnonymous]
        public async Task<IActionResult> SearchContacts([FromQuery] string term)
        {
            var contacts = await _dbContext.Contacts
                .Where(c => c.Name.Contains(term) ||
                            c.Email.Contains(term) ||
                            c.Phone.Contains(term))
                .ToListAsync();

            return Ok(contacts);
        }

   

        [HttpPost("add")]

        public async Task<IActionResult> AddContact([FromBody] ContactDto contactDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var contact = new Contact
            {
                Name = contactDto.Name,
                Email = contactDto.EmailAddress,
                Phone = contactDto.PhoneNumber,
                PhotoUrl = contactDto.PhotoURL
            };

            _dbContext.Contacts.Add(contact);
            await _dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetContactById), new { id = contact.Id }, contact);
        }







        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateContact(int id, [FromBody] ContactDto updatedContactDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var contact = await _dbContext.Contacts.FindAsync(id);
            if (contact == null)
                return NotFound();

            contact.Name = updatedContactDto.Name;
            contact.Email = updatedContactDto.EmailAddress;
            contact.Phone = updatedContactDto.PhoneNumber;
            contact.PhotoUrl = updatedContactDto.PhotoURL;

            _dbContext.Entry(contact).State = EntityState.Modified;

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ContactExists(id))
                    return NotFound();
                throw;
            }

            return NoContent();
        }







        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteContact(int id)
        {
            var contact = await _dbContext.Contacts.FindAsync(id);
            if (contact == null)
                return NotFound();

            _dbContext.Contacts.Remove(contact);
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }

        [HttpPatch("photo/{id}")]
        public async Task<IActionResult> UpdateContactPhoto(int id, [FromBody] string photoUrl)
        {
            var contact = await _dbContext.Contacts.FindAsync(id);
            if (contact == null)
                return NotFound();

            contact.PhotoUrl = photoUrl;
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }

        private bool ContactExists(int id)
        {
            return _dbContext.Contacts.Any(c => c.Id == id);
        }
    }
}
