import { Link, Outlet } from 'react-router'

export function RootLayout() {
  return (
    <div className="min-h-screen bg-slate-50 text-slate-900">
      <header className="bg-white shadow-sm">
        <div className="mx-auto flex max-w-4xl items-center px-4 py-4">
          <Link to="/" className="rounded text-lg font-semibold text-indigo-700 hover:underline focus:outline-none focus-visible:ring-2 focus-visible:ring-indigo-500">
            Event Registration
          </Link>
        </div>
      </header>
      <main className="mx-auto max-w-4xl px-4 py-8">
        <Outlet />
      </main>
    </div>
  )
}
