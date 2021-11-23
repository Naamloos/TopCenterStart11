use std::io;
#[cfg(windows)] use winres::WindowsResource;

fn main() -> io::Result<()> {
    #[cfg(windows)] {
        WindowsResource::new()
            // This path can be absolute, or relative to your crate root.
            .set_icon("assets/icon.ico")
            .compile()?;
    }
    Ok(())
}