using ApiFuncional.Data;
using ApiFuncional.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiFuncional.Controllers
{
    //[Authorize]
    [ApiController]
    //[ApiConventionType(typeof(DefaultApiConventions))]
    [Route("api/produtos")]
    public class ProdutosControler : ControllerBase
    {
        private readonly ApiDbContext _context;

        public ProdutosControler(ApiDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<Produto>>> GetProdutos()
        {
            if (_context.Produtos == null) return NotFound();

            return await _context.Produtos.ToListAsync();
        }

        [AllowAnonymous]
        [HttpGet("{id:int}")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Produto>> GetProduto(int id)
        {
            //throw new Exception("Sample exception.");

            if (_context.Produtos == null) return NotFound();

            var produto = await _context.Produtos.FindAsync(id);

            if (produto == null) return NotFound();

            return produto;
        }

        [HttpPost]
        //[ProducesResponseType(StatusCodes.Status201Created)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //[ApiConventionMethod(typeof(DefaultApiConventions),nameof(DefaultApiConventions.Post))]
        public async Task<ActionResult<Produto>> PostProduto(Produto produto)
        {
            if (_context.Produtos == null) return Problem("Erro ao criar um produto,contate o suporte!");

            if (!ModelState.IsValid)
            {
                //return BadRequest(ModelState);

                //return ValidationProblem(ModelState);

                return ValidationProblem(new ValidationProblemDetails(ModelState) { Title = "Um ou mais erros de validação ocorreram!" });
            }

            _context.Produtos.Add(produto);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProduto), new { id = produto.Id }, produto);
        }

        [HttpPut("{id:int}")]
        //[ProducesResponseType(StatusCodes.Status204NoContent)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        //[ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Put))]
        public async Task<IActionResult> PutProduto(int id, Produto produto)
        {
            if (id != produto.Id) return BadRequest();

            if (!ModelState.IsValid) ValidationProblem(ModelState);

            _context.Entry(produto).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProdutoExists(id)) return NotFound();

                throw;
            }

            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:int}")]
        //[ProducesResponseType(StatusCodes.Status204NoContent)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteProduto(int id)
        {
            if (_context.Produtos == null) return NotFound();

            var produto = await _context.Produtos.FindAsync(id);

            if (produto == null) return NotFound();

            _context.Produtos.Remove(produto);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProdutoExists(int id)
        {
            return (_context.Produtos?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}